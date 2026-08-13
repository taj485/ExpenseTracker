using ExpenseTracker.Application.DTO;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Interfaces;
using MediatR;

namespace ExpenseTracker.Application.Queries.GetReceiptExtractionStatus
{
    /// <summary>
    /// Single source of truth for extraction status, shared by the SignalR push and the fallback poll
    /// so the two can never disagree about shape or authorization.
    /// </summary>
    public class GetReceiptExtractionStatusQueryHandler : IRequestHandler<GetReceiptExtractionStatusQuery, ReceiptExtractionStatusDto>
    {
        private readonly IReceiptExtractionJobReader _jobReader;
        private readonly IReceiptExtractionResultReader _resultReader;
        private readonly ICurrentUserProvider _currentUserProvider;

        public GetReceiptExtractionStatusQueryHandler(
            IReceiptExtractionJobReader jobReader,
            IReceiptExtractionResultReader resultReader,
            ICurrentUserProvider currentUserProvider)
        {
            _jobReader = jobReader;
            _resultReader = resultReader;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<ReceiptExtractionStatusDto> Handle(GetReceiptExtractionStatusQuery request, CancellationToken cancellationToken)
        {
            var userId = request.UserId ?? (await _currentUserProvider.GetOrProvisionAsync(cancellationToken)).Id;

            var job = await _jobReader.GetByIdAsync(request.JobId, cancellationToken);

            // Same 404 for "no such job" and "not yours" — never confirm that someone else's job exists.
            if (job is null || job.UserId != userId)
                throw new NotFoundException($"Extraction job with id {request.JobId} was not found");

            var result = await _resultReader.GetAsync(request.JobId, cancellationToken);

            // The analyser has not created its row yet; the request is in flight on the topic.
            if (result is null)
            {
                return new ReceiptExtractionStatusDto
                {
                    JobId = request.JobId,
                    Status = ReceiptExtractionStatus.Pending.ToString(),
                };
            }

            return new ReceiptExtractionStatusDto
            {
                JobId = result.JobId,
                Status = result.Status.ToString(),
                Error = result.Error,
                Items = result.Items.Select(i => new ExtractedExpenseDto
                {
                    Amount = i.Amount,
                    Category = i.Category.ToString(),
                    Description = i.Description,
                    Date = i.Date,
                    Quantity = i.Quantity,
                    Merchant = i.Merchant,
                }).ToList(),
            };
        }
    }
}

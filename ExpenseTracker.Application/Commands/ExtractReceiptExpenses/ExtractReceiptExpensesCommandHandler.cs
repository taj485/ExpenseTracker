using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;
using MediatR;

namespace ExpenseTracker.Application.Commands.ExtractReceiptExpenses
{
    public class ExtractReceiptExpensesCommandHandler : IRequestHandler<ExtractReceiptExpensesCommand, ExtractReceiptExpensesResult>
    {
        private readonly IExpenseTableReader _expenseTableReader;
        private readonly IReceiptImageStore _receiptImageStore;
        private readonly IReceiptExtractionRequestPublisher _publisher;
        private readonly IReceiptExtractionJobWriter _jobWriter;
        private readonly ICurrentUserProvider _currentUserProvider;

        public ExtractReceiptExpensesCommandHandler(
            IExpenseTableReader expenseTableReader,
            IReceiptImageStore receiptImageStore,
            IReceiptExtractionRequestPublisher publisher,
            IReceiptExtractionJobWriter jobWriter,
            ICurrentUserProvider currentUserProvider)
        {
            _expenseTableReader = expenseTableReader;
            _receiptImageStore = receiptImageStore;
            _publisher = publisher;
            _jobWriter = jobWriter;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<ExtractReceiptExpensesResult> Handle(ExtractReceiptExpensesCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await _currentUserProvider.GetOrProvisionAsync(cancellationToken);

            if (!await _expenseTableReader.IsMemberAsync(request.ExpenseTableId, currentUser.Id, cancellationToken))
                throw new NotFoundException($"Expense table with id {request.ExpenseTableId} was not found");

            var tempReference = await _receiptImageStore.UploadTempAsync(request.ImageBytes, request.ContentType, cancellationToken);
            var imageUrl = _receiptImageStore.GenerateTempReadSasUri(tempReference);

            var jobId = Guid.NewGuid();

            // Persist before publishing: the analyser can finish fast enough that the completion event
            // races the write, and the status query authorizes off this row.
            await _jobWriter.AddAsync(ReceiptExtractionJob.Create(jobId, currentUser.Id, request.ExpenseTableId), cancellationToken);

            await _publisher.PublishAsync(jobId, imageUrl, request.ContentType, cancellationToken);

            return new ExtractReceiptExpensesResult(jobId, tempReference);
        }
    }
}

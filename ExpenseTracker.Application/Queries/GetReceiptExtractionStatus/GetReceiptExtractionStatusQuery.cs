using ExpenseTracker.Application.DTO;
using MediatR;

namespace ExpenseTracker.Application.Queries.GetReceiptExtractionStatus
{
    /// <param name="UserId">
    /// When set, authorization is checked against this user instead of the HTTP principal. The Kafka
    /// completion consumer has no HTTP context, so it supplies the job owner directly.
    /// </param>
    public record GetReceiptExtractionStatusQuery(Guid JobId, int? UserId = null) : IRequest<ReceiptExtractionStatusDto>;
}

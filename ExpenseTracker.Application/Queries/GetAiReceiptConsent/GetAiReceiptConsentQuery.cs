using ExpenseTracker.Application.DTO;
using MediatR;

namespace ExpenseTracker.Application.Queries.GetAiReceiptConsent
{
    public record GetAiReceiptConsentQuery : IRequest<AiReceiptConsentDto>;
}

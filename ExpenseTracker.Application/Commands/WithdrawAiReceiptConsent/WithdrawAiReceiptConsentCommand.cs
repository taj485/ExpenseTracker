using MediatR;

namespace ExpenseTracker.Application.Commands.WithdrawAiReceiptConsent
{
    public record WithdrawAiReceiptConsentCommand : IRequest<Unit>;
}

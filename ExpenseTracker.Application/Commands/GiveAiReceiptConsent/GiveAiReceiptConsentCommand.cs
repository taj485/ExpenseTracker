using MediatR;

namespace ExpenseTracker.Application.Commands.GiveAiReceiptConsent
{
    public record GiveAiReceiptConsentCommand(int Version) : IRequest<Unit>;
}

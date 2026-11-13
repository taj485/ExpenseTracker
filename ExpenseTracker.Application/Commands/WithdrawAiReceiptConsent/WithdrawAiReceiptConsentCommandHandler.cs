using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Interfaces;
using MediatR;

namespace ExpenseTracker.Application.Commands.WithdrawAiReceiptConsent
{
    public class WithdrawAiReceiptConsentCommandHandler : IRequestHandler<WithdrawAiReceiptConsentCommand, Unit>
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IUserWriter _userWriter;

        public WithdrawAiReceiptConsentCommandHandler(ICurrentUserProvider currentUserProvider, IUserWriter userWriter)
        {
            _currentUserProvider = currentUserProvider;
            _userWriter = userWriter;
        }

        public async Task<Unit> Handle(WithdrawAiReceiptConsentCommand request, CancellationToken cancellationToken)
        {
            var user = await _currentUserProvider.GetOrProvisionAsync(cancellationToken);
            user.WithdrawAiReceiptConsent();
            await _userWriter.UpdateAsync(user);

            return Unit.Value;
        }
    }
}

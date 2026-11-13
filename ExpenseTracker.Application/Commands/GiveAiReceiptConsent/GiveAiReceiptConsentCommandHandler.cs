using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Exceptions;
using ExpenseTracker.Domain.Interfaces;
using MediatR;

namespace ExpenseTracker.Application.Commands.GiveAiReceiptConsent
{
    public class GiveAiReceiptConsentCommandHandler : IRequestHandler<GiveAiReceiptConsentCommand, Unit>
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IUserWriter _userWriter;
        private readonly TimeProvider _timeProvider;

        public GiveAiReceiptConsentCommandHandler(ICurrentUserProvider currentUserProvider, IUserWriter userWriter, TimeProvider timeProvider)
        {
            _currentUserProvider = currentUserProvider;
            _userWriter = userWriter;
            _timeProvider = timeProvider;
        }

        public async Task<Unit> Handle(GiveAiReceiptConsentCommand request, CancellationToken cancellationToken)
        {
            if (request.Version != AiReceiptConsent.CurrentVersion)
                throw new DomainException(AiReceiptConsent.OutOfDateMessage);

            var user = await _currentUserProvider.GetOrProvisionAsync(cancellationToken);
            user.GiveAiReceiptConsent(request.Version, _timeProvider.GetUtcNow());
            await _userWriter.UpdateAsync(user);

            return Unit.Value;
        }
    }
}

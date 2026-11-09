using ExpenseTracker.Domain.Exceptions;
using ExpenseTracker.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.Commands.DeleteAccount
{
    public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, Unit>
    {
        public const string IdentityProviderFailureMessage = "Couldn't delete your account. Please try again.";

        private readonly ICurrentUserService _currentUserService;
        private readonly IAccountWriter _accountWriter;
        private readonly IReceiptImageStore _receiptImageStore;
        private readonly IIdentityProviderAdmin _identityProviderAdmin;
        private readonly ILogger<DeleteAccountCommandHandler> _logger;

        public DeleteAccountCommandHandler(ICurrentUserService currentUserService, IAccountWriter accountWriter,
            IReceiptImageStore receiptImageStore, IIdentityProviderAdmin identityProviderAdmin, ILogger<DeleteAccountCommandHandler> logger)
        {
            _currentUserService = currentUserService;
            _accountWriter = accountWriter;
            _receiptImageStore = receiptImageStore;
            _identityProviderAdmin = identityProviderAdmin;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            // Resolved by subject only: deleting must never provision a user.
            var subject = _currentUserService.GetCurrentUserSubject();

            var imageReferences = await _accountWriter.DeleteAccountDataAsync(subject, cancellationToken);

            // The database part is committed. Leftover photos are unreachable orphans, so a failed
            // photo delete is logged rather than failing the request.
            foreach (var imageReference in imageReferences)
            {
                try
                {
                    await _receiptImageStore.DeleteAsync(imageReference, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Couldn't delete receipt image {ImageReference} for a deleted account", imageReference);
                }
            }

            try
            {
                await _identityProviderAdmin.DeleteUserAsync(subject, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Couldn't delete the identity-provider user for a deleted account");
                throw new ExternalServiceException(IdentityProviderFailureMessage, ex);
            }

            return Unit.Value;
        }
    }
}

using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;

namespace ExpenseTracker.Application.Services
{
    public class CurrentUserProvider : ICurrentUserProvider
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserReader _userReader;
        private readonly IUserWriter _userWriter;
        private readonly IDeletedAccountReader _deletedAccountReader;

        public CurrentUserProvider(ICurrentUserService currentUserService, IUserReader userReader, IUserWriter userWriter,
            IDeletedAccountReader deletedAccountReader)
        {
            _currentUserService = currentUserService;
            _userReader = userReader;
            _userWriter = userWriter;
            _deletedAccountReader = deletedAccountReader;
        }

        public async Task<User> GetOrProvisionAsync(CancellationToken ct)
        {
            var subject = _currentUserService.GetCurrentUserSubject();
            var email = _currentUserService.GetCurrentUserEmail();
            var user = await _userReader.GetByUserIdpAsync(subject, ct);

            if (user is null)
            {
                // A token issued before its account was deleted stays valid until it expires;
                // it must not bring the account back as a new, empty user.
                if (await _deletedAccountReader.IsDeletedAsync(subject, ct))
                    throw new UnauthorizedAccessException("This account has been deleted.");

                user = User.Create(subject, email);
                await _userWriter.AddAsync(user, ct);
                return user;
            }

            if (email is not null && email.Trim().ToLowerInvariant() != user.Email)
            {
                user.UpdateEmail(email);
                await _userWriter.UpdateAsync(user);
            }

            return user;
        }
    }
}
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;

namespace ExpenseTracker.Application.Services
{
    public class CurrentUserProvider : ICurrentUserProvider
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserReader _userReader;
        private readonly IUserWriter _userWriter;

        public CurrentUserProvider(ICurrentUserService currentUserService, IUserReader userReader, IUserWriter userWriter)
        {
            _currentUserService = currentUserService;
            _userReader = userReader;
            _userWriter = userWriter;
        }

        public async Task<User> GetOrProvisionAsync(CancellationToken ct)
        {
            var subject = _currentUserService.GetCurrentUserSubject();
            var email = _currentUserService.GetCurrentUserEmail();
            var user = await _userReader.GetByUserIdpAsync(subject, ct);

            if (user is null)
            {
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
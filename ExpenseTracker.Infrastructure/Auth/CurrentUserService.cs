using ExpenseTracker.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ExpenseTracker.Infrastructure.Auth
{
    public class CurrentUserService : ICurrentUserService
    {
        private const string EmailClaim = "https://api.expensetracker/email";

        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentUserSubject()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var subject = user?.FindFirst("sub")?.Value ?? user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(subject))
                throw new UnauthorizedAccessException("No authenticated user found.");

            return subject;
        }

        public string? GetCurrentUserEmail()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(EmailClaim)?.Value;
        }
    }
}
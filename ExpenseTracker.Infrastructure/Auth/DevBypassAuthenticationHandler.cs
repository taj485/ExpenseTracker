// AI dev mode: authenticates every request as one fixed local user so the app can be
// driven without an Auth0 login. Registered only when Auth0:DevBypass is set (the "ai"
// launch profile does this) and the environment is Development — see AddAuth0Authentication.

using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExpenseTracker.Infrastructure.Auth
{
    public sealed class DevBypassAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "DevBypass";

        // Fixed, so every AI dev mode run reuses the same local user row.
        private const string DevSubject = "dev|local-design-user";
        private const string DevEmail = "testaccount1@recave.co.uk";

        // CurrentUserService reads email from this namespaced claim, not ClaimTypes.Email.
        private const string EmailClaimType = "https://api.expensetracker/email";

        public DevBypassAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, DevSubject),
                new Claim("sub", DevSubject),
                new Claim(EmailClaimType, DevEmail),
            };

            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}

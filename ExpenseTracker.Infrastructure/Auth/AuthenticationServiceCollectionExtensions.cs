using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ExpenseTracker.Infrastructure.Auth
{
    public static class AuthenticationServiceCollectionExtensions
    {
        public static IServiceCollection AddAuth0Authentication(this IServiceCollection services, IConfiguration configuration) =>
            services.AddAuth0Authentication(configuration, Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));

        public static IServiceCollection AddAuth0Authentication(this IServiceCollection services, IConfiguration configuration, string? environmentName)
        {
            var domain = configuration["Auth0:Domain"];
            var audience = configuration["Auth0:Audience"];

            // AI dev mode: authenticates every request as one fixed local user so the app can be
            // driven without an Auth0 login. Opt-in via Auth0:DevBypass (the "ai" launch profile
            // sets it), and refused outside Development so a stray setting fails startup instead
            // of silently disabling authentication.
            if (configuration.GetValue<bool>("Auth0:DevBypass"))
            {
                if (!string.Equals(environmentName, "Development", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Auth0:DevBypass is only allowed when ASPNETCORE_ENVIRONMENT is Development.");

                services
                    .AddAuthentication(DevBypassAuthenticationHandler.SchemeName)
                    .AddScheme<AuthenticationSchemeOptions, DevBypassAuthenticationHandler>(
                        DevBypassAuthenticationHandler.SchemeName, _ => { });

                services.AddAuthorization();

                return services;
            }

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = $"https://{domain}/";
                    options.Audience = audience;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = $"https://{domain}/",
                        ValidateAudience = true,
                        ValidAudience = audience,
                        ValidateLifetime = true,
                    };
                });

            services.AddAuthorization();

            return services;
        }
    }
}

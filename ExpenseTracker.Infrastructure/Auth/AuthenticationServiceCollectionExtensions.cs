using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ExpenseTracker.Infrastructure.Auth
{
    public static class AuthenticationServiceCollectionExtensions
    {
        /// <summary>Hub routes that accept the token as a query parameter.</summary>
        private const string HubPathPrefix = "/hubs";

        public static IServiceCollection AddAuth0Authentication(this IServiceCollection services, IConfiguration configuration)
        {
            var domain = configuration["Auth0:Domain"];
            var audience = configuration["Auth0:Audience"];

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

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            // A browser cannot set an Authorization header on a WebSocket handshake, so
                            // SignalR sends the token as ?access_token=. Without this the hub 401s while
                            // the REST API keeps working — the client silently degrades to polling and
                            // nothing looks broken.
                            var accessToken = context.Request.Query["access_token"];

                            if (!string.IsNullOrEmpty(accessToken) &&
                                context.HttpContext.Request.Path.StartsWithSegments(HubPathPrefix))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        },
                    };
                });

            services.AddAuthorization();

            return services;
        }
    }
}

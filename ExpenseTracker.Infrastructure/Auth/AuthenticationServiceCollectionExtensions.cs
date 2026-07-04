using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ExpenseTracker.Infrastructure.Auth
{
    public static class AuthenticationServiceCollectionExtensions
    {
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
                });

            services.AddAuthorization();

            return services;
        }
    }
}

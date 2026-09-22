using ExpenseTracker.Infrastructure.Auth;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ExpenseTracker.Tests.Infrastructure
{
    /// <summary>
    /// AI dev mode swaps real authentication for a fixed local user, so the guard around it
    /// is the thing worth pinning: it must only ever switch on in Development.
    /// </summary>
    public class AuthenticationRegistrationTests
    {
        private static IServiceCollection Register(string environmentName, bool devBypass)
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Auth0:Domain"] = "example.auth0.com",
                    ["Auth0:Audience"] = "https://api.example",
                    ["Auth0:DevBypass"] = devBypass.ToString(),
                })
                .Build();

            return new ServiceCollection().AddAuth0Authentication(configuration, environmentName);
        }

        private static string? DefaultSchemeOf(IServiceCollection services) =>
            services.BuildServiceProvider().GetRequiredService<IOptions<AuthenticationOptions>>().Value.DefaultScheme;

        [Fact]
        public void DevBypassInDevelopmentUsesTheDevScheme()
        {
            DefaultSchemeOf(Register("Development", devBypass: true))
                .Should().Be(DevBypassAuthenticationHandler.SchemeName);
        }

        [Fact]
        public void DevBypassOutsideDevelopmentRefusesToStart()
        {
            var register = () => Register("Production", devBypass: true);

            register.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void WithoutDevBypassDevelopmentUsesAuth0Jwt()
        {
            DefaultSchemeOf(Register("Development", devBypass: false))
                .Should().Be(JwtBearerDefaults.AuthenticationScheme);
        }
    }
}

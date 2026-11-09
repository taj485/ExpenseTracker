using System.Net;
using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Infrastructure;
using ExpenseTracker.Infrastructure.Auth;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ExpenseTracker.Tests.Infrastructure
{
    public class Auth0IdentityProviderAdminTests
    {
        private sealed class StubHandler : HttpMessageHandler
        {
            private readonly HttpStatusCode _deleteStatus;
            public List<HttpRequestMessage> Requests { get; } = new();

            public StubHandler(HttpStatusCode deleteStatus) => _deleteStatus = deleteStatus;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Requests.Add(request);
                if (request.Method == HttpMethod.Post)
                {
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("{\"access_token\":\"m2m-token\"}", System.Text.Encoding.UTF8, "application/json")
                    });
                }

                return Task.FromResult(new HttpResponseMessage(_deleteStatus));
            }
        }

        private static Auth0IdentityProviderAdmin CreateAdmin(StubHandler handler) =>
            new(new HttpClient(handler), Options.Create(new Auth0ManagementOptions
            {
                Domain = "tenant.auth0.com",
                ClientId = "client",
                ClientSecret = "secret"
            }));

        [Fact]
        public async Task DeleteUserAsync_DeletesTheEscapedSubjectWithAManagementToken()
        {
            var handler = new StubHandler(HttpStatusCode.NoContent);

            await CreateAdmin(handler).DeleteUserAsync("auth0|alex");

            var delete = handler.Requests.Single(r => r.Method == HttpMethod.Delete);
            delete.RequestUri!.AbsoluteUri.Should().Be("https://tenant.auth0.com/api/v2/users/auth0%7Calex");
            delete.Headers.Authorization!.Parameter.Should().Be("m2m-token");
        }

        [Fact]
        public async Task DeleteUserAsync_WhenTheUserIsAlreadyGone_DoesNotThrow()
        {
            var act = () => CreateAdmin(new StubHandler(HttpStatusCode.NotFound)).DeleteUserAsync("auth0|alex");

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task DeleteUserAsync_WhenAuth0Fails_Throws()
        {
            var act = () => CreateAdmin(new StubHandler(HttpStatusCode.InternalServerError)).DeleteUserAsync("auth0|alex");

            await act.Should().ThrowAsync<HttpRequestException>();
        }

        [Fact]
        public async Task DeleteUserAsync_WhenNotConfigured_Throws()
        {
            var admin = new Auth0IdentityProviderAdmin(new HttpClient(new StubHandler(HttpStatusCode.NoContent)),
                Options.Create(new Auth0ManagementOptions { Domain = "tenant.auth0.com" }));

            var act = () => admin.DeleteUserAsync("auth0|alex");

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Theory]
        [InlineData("true", typeof(NoOpIdentityProviderAdmin))]
        [InlineData("false", typeof(Auth0IdentityProviderAdmin))]
        public void AddIdentityProviderAdmin_UsesTheNoOpOnlyInAiDevMode(string devBypass, Type expected)
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> { ["Auth0:DevBypass"] = devBypass })
                .Build();

            var provider = new ServiceCollection()
                .AddIdentityProviderAdmin(configuration)
                .BuildServiceProvider();

            using var scope = provider.CreateScope();
            scope.ServiceProvider.GetRequiredService<IIdentityProviderAdmin>().Should().BeOfType(expected);
        }
    }
}

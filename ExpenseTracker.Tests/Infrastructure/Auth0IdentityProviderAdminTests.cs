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
            private readonly HttpStatusCode _tokenStatus;
            private readonly string _tokenBody;
            public List<HttpRequestMessage> Requests { get; } = new();
            public string? TokenRequestBody { get; private set; }

            public StubHandler(HttpStatusCode deleteStatus, HttpStatusCode tokenStatus = HttpStatusCode.OK,
                string tokenBody = "{\"access_token\":\"m2m-token\"}")
            {
                _deleteStatus = deleteStatus;
                _tokenStatus = tokenStatus;
                _tokenBody = tokenBody;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Requests.Add(request);
                if (request.Method == HttpMethod.Post)
                {
                    TokenRequestBody = await request.Content!.ReadAsStringAsync(cancellationToken);
                    return new HttpResponseMessage(_tokenStatus)
                    {
                        Content = new StringContent(_tokenBody, System.Text.Encoding.UTF8, "application/json")
                    };
                }

                return new HttpResponseMessage(_deleteStatus);
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
        public async Task DeleteUserAsync_EscapesSlashesInTheSubject_AndUsesTheBearerScheme()
        {
            var handler = new StubHandler(HttpStatusCode.NoContent);

            await CreateAdmin(handler).DeleteUserAsync("oauth2/a?b");

            var delete = handler.Requests.Single(r => r.Method == HttpMethod.Delete);
            delete.RequestUri!.OriginalString.Should().Be("https://tenant.auth0.com/api/v2/users/oauth2%2Fa%3Fb");
            delete.Headers.Authorization!.Scheme.Should().Be("Bearer");
        }

        [Fact]
        public async Task DeleteUserAsync_RequestsAClientCredentialsTokenForTheManagementApi()
        {
            var handler = new StubHandler(HttpStatusCode.NoContent);

            await CreateAdmin(handler).DeleteUserAsync("auth0|alex");

            var token = handler.Requests.Single(r => r.Method == HttpMethod.Post);
            token.RequestUri!.AbsoluteUri.Should().Be("https://tenant.auth0.com/oauth/token");
            using var body = System.Text.Json.JsonDocument.Parse(handler.TokenRequestBody!);
            body.RootElement.GetProperty("grant_type").GetString().Should().Be("client_credentials");
            body.RootElement.GetProperty("client_id").GetString().Should().Be("client");
            body.RootElement.GetProperty("client_secret").GetString().Should().Be("secret");
            body.RootElement.GetProperty("audience").GetString().Should().Be("https://tenant.auth0.com/api/v2/");
        }

        [Fact]
        public async Task DeleteUserAsync_WhenTheTokenRequestFails_ThrowsWithoutDeleting()
        {
            var handler = new StubHandler(HttpStatusCode.NoContent, HttpStatusCode.Unauthorized);

            var act = () => CreateAdmin(handler).DeleteUserAsync("auth0|alex");

            await act.Should().ThrowAsync<HttpRequestException>();
            handler.Requests.Should().NotContain(r => r.Method == HttpMethod.Delete);
        }

        [Fact]
        public async Task DeleteUserAsync_WhenAuth0ReturnsNoToken_ThrowsWithoutDeleting()
        {
            var handler = new StubHandler(HttpStatusCode.NoContent, tokenBody: "{}");

            var act = () => CreateAdmin(handler).DeleteUserAsync("auth0|alex");

            await act.Should().ThrowAsync<InvalidOperationException>();
            handler.Requests.Should().NotContain(r => r.Method == HttpMethod.Delete);
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
        [InlineData(null, "client", "secret")]
        [InlineData("tenant.auth0.com", null, "secret")]
        [InlineData("tenant.auth0.com", "client", null)]
        [InlineData("tenant.auth0.com", " ", "secret")]
        public async Task DeleteUserAsync_WhenAnyManagementSettingIsMissing_ThrowsWithoutCallingAuth0(
            string? domain, string? clientId, string? clientSecret)
        {
            var handler = new StubHandler(HttpStatusCode.NoContent);
            var admin = new Auth0IdentityProviderAdmin(new HttpClient(handler),
                Options.Create(new Auth0ManagementOptions { Domain = domain, ClientId = clientId, ClientSecret = clientSecret }));

            var act = () => admin.DeleteUserAsync("auth0|alex");

            await act.Should().ThrowAsync<InvalidOperationException>();
            handler.Requests.Should().BeEmpty();
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

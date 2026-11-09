using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace ExpenseTracker.Tests.Api
{
    /// <summary>
    /// Authenticates a request as the subject in the X-Test-Subject header; no header means no user.
    /// </summary>
    public class TestSubjectAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "TestSubject";
        public const string HeaderName = "X-Test-Subject";

        public TestSubjectAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(HeaderName, out var subject) || string.IsNullOrEmpty(subject))
                return Task.FromResult(AuthenticateResult.NoResult());

            var identity = new ClaimsIdentity(new[] { new Claim("sub", subject.ToString()) }, SchemeName);
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    public class AccountControllerTests
    {
        private const string Subject = "auth0|alex";

        private readonly Mock<IAccountWriter> _accountWriter = new();
        private readonly Mock<IDeletedAccountReader> _deletedAccountReader = new();
        private readonly Mock<IReceiptImageStore> _imageStore = new();
        private readonly Mock<IIdentityProviderAdmin> _identityProviderAdmin = new();
        private readonly Mock<IUserReader> _userReader = new();
        private readonly Mock<IUserWriter> _userWriter = new();
        private readonly Mock<IExpenseTableReader> _expenseTableReader = new();

        private HttpClient CreateClient(string? subject = Subject)
        {
            var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.UseSetting("Database:AutoMigrate", "false");
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication()
                        .AddScheme<AuthenticationSchemeOptions, TestSubjectAuthenticationHandler>(TestSubjectAuthenticationHandler.SchemeName, _ => { });
                    services.PostConfigure<AuthenticationOptions>(options =>
                    {
                        options.DefaultScheme = TestSubjectAuthenticationHandler.SchemeName;
                        options.DefaultAuthenticateScheme = TestSubjectAuthenticationHandler.SchemeName;
                        options.DefaultChallengeScheme = TestSubjectAuthenticationHandler.SchemeName;
                    });

                    services.AddScoped(_ => _accountWriter.Object);
                    services.AddScoped(_ => _deletedAccountReader.Object);
                    services.AddScoped(_ => _imageStore.Object);
                    services.AddScoped(_ => _identityProviderAdmin.Object);
                    services.AddScoped(_ => _userReader.Object);
                    services.AddScoped(_ => _userWriter.Object);
                    services.AddScoped(_ => _expenseTableReader.Object);
                });
            });

            var client = factory.CreateClient();
            if (subject is not null)
                client.DefaultRequestHeaders.Add(TestSubjectAuthenticationHandler.HeaderName, subject);
            return client;
        }

        [Fact]
        public async Task DeleteAccount_WithoutToken_Returns401()
        {
            var client = CreateClient(subject: null);

            var response = await client.DeleteAsync("/api/account");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            _accountWriter.Verify(x => x.DeleteAccountDataAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task S1_DeleteAccount_Returns204_AndDeletesPhotosAndTheAuth0User()
        {
            _accountWriter.Setup(x => x.DeleteAccountDataAsync(Subject, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { "tesco.jpg" });
            var client = CreateClient();

            var response = await client.DeleteAsync("/api/account");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            _imageStore.Verify(x => x.DeleteAsync("tesco.jpg", It.IsAny<CancellationToken>()), Times.Once);
            _identityProviderAdmin.Verify(x => x.DeleteUserAsync(Subject, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task S6_DeleteAccount_WhenAuth0DeleteFails_Returns502WithError()
        {
            _accountWriter.Setup(x => x.DeleteAccountDataAsync(Subject, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<string>());
            _identityProviderAdmin.Setup(x => x.DeleteUserAsync(Subject, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Auth0 is down"));
            var client = CreateClient();

            var response = await client.DeleteAsync("/api/account");

            response.StatusCode.Should().Be(HttpStatusCode.BadGateway);
            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            json.RootElement.GetProperty("error").GetString().Should().Be("Couldn't delete your account. Please try again.");
        }

        [Fact]
        public async Task S6_DeleteAccount_RetryAfterPartialFailure_Returns204()
        {
            _accountWriter.Setup(x => x.DeleteAccountDataAsync(Subject, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<string>());
            _identityProviderAdmin.SetupSequence(x => x.DeleteUserAsync(Subject, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Auth0 is down"))
                .Returns(Task.CompletedTask);
            var client = CreateClient();

            var first = await client.DeleteAsync("/api/account");
            var retry = await client.DeleteAsync("/api/account");

            first.StatusCode.Should().Be(HttpStatusCode.BadGateway);
            retry.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task S7_OldTokenOfADeletedAccount_Returns401_AndCreatesNoUser()
        {
            _userReader.Setup(x => x.GetByUserIdpAsync(Subject, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);
            _deletedAccountReader.Setup(x => x.IsDeletedAsync(Subject, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var client = CreateClient();

            var response = await client.GetAsync("/api/expensetable");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            _userWriter.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}

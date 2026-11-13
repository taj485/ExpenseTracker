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

    /// <summary>A clock that always reads the given time.</summary>
    public class FixedTimeProvider : TimeProvider
    {
        private DateTimeOffset _now;

        public FixedTimeProvider(DateTimeOffset now) => _now = now;

        public void SetNow(DateTimeOffset now) => _now = now;

        public override DateTimeOffset GetUtcNow() => _now;
    }

    public class AccountControllerTests
    {
        private static readonly DateTimeOffset Now = new(2026, 11, 12, 10, 15, 0, TimeSpan.Zero);
        private readonly FixedTimeProvider _timeProvider = new(Now);
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
                    services.AddSingleton<TimeProvider>(_timeProvider);
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

        private User SeedUser()
        {
            var user = User.Create(Subject, "alex@example.com");
            _userReader.Setup(x => x.GetByUserIdpAsync(Subject, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            return user;
        }

        private static async Task<JsonElement> ReadJsonAsync(HttpResponseMessage response)
        {
            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return json.RootElement.Clone();
        }

        private static HttpContent VersionBody(int version) =>
            new StringContent("{\"version\":" + version + "}", System.Text.Encoding.UTF8, "application/json");

        [Fact]
        public async Task S1_GetAiReceiptConsent_WhenNeverGiven_ReturnsNotGiven()
        {
            SeedUser();
            var client = CreateClient();

            var response = await client.GetAsync("/api/account/ai-receipt-consent");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var body = await ReadJsonAsync(response);
            body.GetProperty("given").GetBoolean().Should().BeFalse();
            body.GetProperty("version").ValueKind.Should().Be(JsonValueKind.Null);
            body.GetProperty("givenAt").ValueKind.Should().Be(JsonValueKind.Null);
        }

        [Fact]
        public async Task S2_PutAiReceiptConsent_ThenGet_ShowsGivenWithVersion1AndTheServerTime()
        {
            var user = SeedUser();
            var client = CreateClient();

            var put = await client.PutAsync("/api/account/ai-receipt-consent", VersionBody(1));
            var get = await client.GetAsync("/api/account/ai-receipt-consent");

            put.StatusCode.Should().Be(HttpStatusCode.NoContent);
            _userWriter.Verify(x => x.UpdateAsync(user), Times.Once);
            var body = await ReadJsonAsync(get);
            body.GetProperty("given").GetBoolean().Should().BeTrue();
            body.GetProperty("version").GetInt32().Should().Be(1);
            body.GetProperty("givenAt").GetString().Should().Be("2026-11-12T10:15:00Z");
        }

        [Fact]
        public async Task S3_PutAiReceiptConsent_Twice_KeepsTheFirstGivenAt()
        {
            var user = SeedUser();
            var client = CreateClient();

            await client.PutAsync("/api/account/ai-receipt-consent", VersionBody(1));
            _timeProvider.SetNow(Now.AddDays(1));
            var second = await client.PutAsync("/api/account/ai-receipt-consent", VersionBody(1));

            second.StatusCode.Should().Be(HttpStatusCode.NoContent);
            user.AiReceiptConsentGivenAt.Should().Be(Now);
        }

        [Fact]
        public async Task PutAiReceiptConsent_WithAnOutOfDateVersion_Returns400AndStoresNothing()
        {
            var user = SeedUser();
            var client = CreateClient();

            var response = await client.PutAsync("/api/account/ai-receipt-consent", VersionBody(2));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var body = await ReadJsonAsync(response);
            body.GetProperty("error").GetString().Should().Be("This consent text is out of date. Please update the app.");
            user.AiReceiptConsentVersion.Should().BeNull();
            user.AiReceiptConsentGivenAt.Should().BeNull();
            _userWriter.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task S5_DeleteAiReceiptConsent_ClearsIt()
        {
            var user = SeedUser();
            user.GiveAiReceiptConsent(1, Now);
            var client = CreateClient();

            var delete = await client.DeleteAsync("/api/account/ai-receipt-consent");
            var get = await client.GetAsync("/api/account/ai-receipt-consent");

            delete.StatusCode.Should().Be(HttpStatusCode.NoContent);
            user.AiReceiptConsentVersion.Should().BeNull();
            user.AiReceiptConsentGivenAt.Should().BeNull();
            (await ReadJsonAsync(get)).GetProperty("given").GetBoolean().Should().BeFalse();
            _userWriter.Verify(x => x.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task DeleteAiReceiptConsent_WhenNothingIsStored_Returns204()
        {
            SeedUser();
            var client = CreateClient();

            var response = await client.DeleteAsync("/api/account/ai-receipt-consent");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task GetAiReceiptConsent_GivenForAnOlderVersion_ReadsAsNotGiven()
        {
            var user = SeedUser();
            user.GiveAiReceiptConsent(0, Now);
            var client = CreateClient();

            var response = await client.GetAsync("/api/account/ai-receipt-consent");

            var body = await ReadJsonAsync(response);
            body.GetProperty("given").GetBoolean().Should().BeFalse();
            body.GetProperty("version").ValueKind.Should().Be(JsonValueKind.Null);
            body.GetProperty("givenAt").ValueKind.Should().Be(JsonValueKind.Null);
        }

        [Fact]
        public async Task GetAiReceiptConsent_OnFirstCall_CreatesTheUserWithNoConsent()
        {
            _userReader.Setup(x => x.GetByUserIdpAsync(Subject, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
            var client = CreateClient();

            var response = await client.GetAsync("/api/account/ai-receipt-consent");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await ReadJsonAsync(response)).GetProperty("given").GetBoolean().Should().BeFalse();
            _userWriter.Verify(x => x.AddAsync(It.Is<User>(u => u.Subject == Subject), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task PutAiReceiptConsent_ForADeletedAccount_Returns401_AndCreatesNoUser()
        {
            _userReader.Setup(x => x.GetByUserIdpAsync(Subject, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
            _deletedAccountReader.Setup(x => x.IsDeletedAsync(Subject, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            var client = CreateClient();

            var response = await client.PutAsync("/api/account/ai-receipt-consent", VersionBody(1));

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            _userWriter.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}

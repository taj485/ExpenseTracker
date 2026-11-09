using System.Text.Json;
using ExpenseTracker.Domain.Exceptions;
using ExpenseTrackerAPI.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace ExpenseTracker.Tests.Api
{
    /// <summary>
    /// The response body is deliberately generic, so the log is the only place the
    /// cause of a 500 survives. These pin that it is actually written.
    /// </summary>
    public class ExceptionHandlingMiddlewareTests
    {
        private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _logger = new();

        private static DefaultHttpContext BuildContext()
        {
            var context = new DefaultHttpContext
            {
                RequestServices = new ServiceCollection().AddOptions().BuildServiceProvider(),
            };
            context.Request.Method = "POST";
            context.Request.Path = "/api/expensetable/1/expenses/extract-receipt";
            context.Response.Body = new MemoryStream();
            return context;
        }

        private static async Task<string> ReadBodyAsync(HttpContext context)
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            return await new StreamReader(context.Response.Body).ReadToEndAsync();
        }

        private void VerifyLoggedError(Exception expected, Times times)
        {
            _logger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                expected,
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), times);
        }

        [Fact]
        public async Task UnhandledException_IsLoggedWithTheException()
        {
            var boom = new InvalidOperationException("Gemini API key is not configured.");
            var context = BuildContext();
            var middleware = new ExceptionHandlingMiddleware(_ => throw boom, _logger.Object);

            await middleware.InvokeAsync(context);

            VerifyLoggedError(boom, Times.Once());
        }

        [Fact]
        public async Task UnhandledException_StillReturnsTheGenericBody()
        {
            var context = BuildContext();
            var middleware = new ExceptionHandlingMiddleware(
                _ => throw new InvalidOperationException("secret internal detail"), _logger.Object);

            await middleware.InvokeAsync(context);

            context.Response.StatusCode.Should().Be(500);

            var body = await ReadBodyAsync(context);
            body.Should().NotContain("secret internal detail");

            using var json = JsonDocument.Parse(body);
            json.RootElement.GetProperty("error").GetString().Should().Be("Something went wrong");
        }

        [Fact]
        public async Task ExpectedExceptions_AreNotLoggedAsErrors()
        {
            var notFound = new NotFoundException("Expense table with id 1 was not found");
            var context = BuildContext();
            var middleware = new ExceptionHandlingMiddleware(_ => throw notFound, _logger.Object);

            await middleware.InvokeAsync(context);

            context.Response.StatusCode.Should().Be(404);
            VerifyLoggedError(notFound, Times.Never());
        }

        [Fact]
        public async Task UnauthorizedAccessException_Returns401()
        {
            var context = BuildContext();
            var middleware = new ExceptionHandlingMiddleware(
                _ => throw new UnauthorizedAccessException("This account has been deleted."), _logger.Object);

            await middleware.InvokeAsync(context);

            context.Response.StatusCode.Should().Be(401);
        }

        [Theory]
        [InlineData(typeof(DomainException), 400, "Domain rule violation")]
        [InlineData(typeof(ForbiddenException), 403, "Forbidden")]
        [InlineData(typeof(NotFoundException), 404, "Not found")]
        [InlineData(typeof(UnauthorizedAccessException), 401, "Unauthorized")]
        [InlineData(typeof(ExternalServiceException), 502, "Bad gateway")]
        public async Task MappedExceptions_ReturnStatusTitleMessageAndJson(Type type, int status, string title)
        {
            var context = BuildContext();
            var ex = type == typeof(ExternalServiceException)
                ? new ExternalServiceException("the message")
                : (Exception)Activator.CreateInstance(type, "the message")!;
            var middleware = new ExceptionHandlingMiddleware(_ => throw ex, _logger.Object);

            await middleware.InvokeAsync(context);

            context.Response.StatusCode.Should().Be(status);
            context.Response.ContentType.Should().StartWith("application/json");
            using var json = JsonDocument.Parse(await ReadBodyAsync(context));
            json.RootElement.GetProperty("status").GetInt32().Should().Be(status);
            json.RootElement.GetProperty("title").GetString().Should().Be(title);
            json.RootElement.GetProperty("error").GetString().Should().Be("the message");
        }

        [Fact]
        public async Task UnhandledException_Returns500WithTitle()
        {
            var context = BuildContext();
            var middleware = new ExceptionHandlingMiddleware(_ => throw new InvalidOperationException("x"), _logger.Object);

            await middleware.InvokeAsync(context);

            context.Response.ContentType.Should().StartWith("application/json");
            using var json = JsonDocument.Parse(await ReadBodyAsync(context));
            json.RootElement.GetProperty("status").GetInt32().Should().Be(500);
            json.RootElement.GetProperty("title").GetString().Should().Be("Internal server error");
        }

        [Fact]
        public async Task ValidationException_Returns400WithFieldErrors()
        {
            var context = BuildContext();
            var failure = new FluentValidation.ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure("Name", "Name is required"),
            });
            var middleware = new ExceptionHandlingMiddleware(_ => throw failure, _logger.Object);

            await middleware.InvokeAsync(context);

            context.Response.StatusCode.Should().Be(400);
            using var json = JsonDocument.Parse(await ReadBodyAsync(context));
            json.RootElement.GetProperty("title").GetString().Should().Be("Validation failed");
            var error = json.RootElement.GetProperty("errors")[0];
            error.GetProperty("field").GetString().Should().Be("Name");
            error.GetProperty("message").GetString().Should().Be("Name is required");
        }

        [Fact]
        public async Task ExternalServiceException_Returns502WithItsMessage_AndIsLogged()
        {
            var failure = new ExternalServiceException("Couldn't delete your account. Please try again.");
            var context = BuildContext();
            var middleware = new ExceptionHandlingMiddleware(_ => throw failure, _logger.Object);

            await middleware.InvokeAsync(context);

            context.Response.StatusCode.Should().Be(502);
            using var json = JsonDocument.Parse(await ReadBodyAsync(context));
            json.RootElement.GetProperty("error").GetString().Should().Be("Couldn't delete your account. Please try again.");
            VerifyLoggedError(failure, Times.Once());
        }
    }
}

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
    }
}

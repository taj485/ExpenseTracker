using ExpenseTracker.Domain.Exceptions;
using FluentValidation;

namespace ExpenseTrackerAPI.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        private sealed record ErrorMapping(Type ExceptionType, int Status, string Title);

        // Checked in order; the first matching type wins.
        private static readonly ErrorMapping[] ErrorMappings =
        {
            new(typeof(DomainException), 400, "Domain rule violation"),
            new(typeof(ForbiddenException), 403, "Forbidden"),
            new(typeof(NotFoundException), 404, "Not found"),
            new(typeof(UnauthorizedAccessException), 401, "Unauthorized"),
            new(typeof(ExternalServiceException), 502, "Bad gateway"),
        };

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                await WriteAsync(context, 400, new
                {
                    status = 400,
                    title = "Validation failed",
                    errors = ex.Errors.Select(e => new
                    {
                        field = e.PropertyName,
                        message = e.ErrorMessage
                    })
                });
            }
            catch (Exception ex)
            {
                await HandleAsync(context, ex);
            }
        }

        private async Task HandleAsync(HttpContext context, Exception ex)
        {
            var mapping = ErrorMappings.FirstOrDefault(m => m.ExceptionType.IsInstanceOfType(ex));
            if (mapping is null)
            {
                await WriteUnhandledAsync(context, ex);
                return;
            }

            if (ex is ExternalServiceException)
            {
                _logger.LogError(ex, "External service failure processing {Method} {Path}",
                    context.Request.Method, context.Request.Path);
            }

            await WriteAsync(context, mapping.Status, new
            {
                status = mapping.Status,
                title = mapping.Title,
                error = ex.Message
            });
        }

        private async Task WriteUnhandledAsync(HttpContext context, Exception ex)
        {
            // The client gets a deliberately generic message; the detail has to
            // land in the logs or a 500 is undiagnosable from the outside.
            _logger.LogError(ex, "Unhandled exception processing {Method} {Path}",
                context.Request.Method, context.Request.Path);

            await WriteAsync(context, 500, new
            {
                status = 500,
                title = "Internal server error",
                error = "Something went wrong"
            });
        }

        private static async Task WriteAsync(HttpContext context, int status, object body)
        {
            context.Response.StatusCode = status;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(body);
        }
    }
}

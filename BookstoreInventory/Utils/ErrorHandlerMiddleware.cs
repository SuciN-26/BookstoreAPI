using FluentValidation;
using System.Net;
using System.Text.Json;

namespace BookstoreInventory.Utils
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Terjadi error saat memproses request {Method} {Path}. Error: {Message}",
                    context.Request.Method,
                    context.Request.Path,
                    ex.Message);

                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (context.Response.HasStarted)
            {
                // Jika response sudah berjalan, kita tidak bisa menulis response lagi
                return;
            }

            var response = context.Response;
            response.ContentType = "application/json";

            int statusCode;
            object errorResponse;

            switch (exception)
            {
                case NotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    errorResponse = new { status = statusCode, error = exception.Message };
                    break;

                case ValidationException validationEx:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    var errors = validationEx.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                    errorResponse = new { status = statusCode, error = "Validation failed.", errors };
                    break;

                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse = new { status = statusCode, error = "Terjadi kesalahan pada server." };
                    break;
            }

            response.StatusCode = statusCode;

            var result = JsonSerializer.Serialize(errorResponse);

            await response.WriteAsync(result);
        }
    }
}

using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using ValidationException = FluentValidation.ValidationException;

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

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int statusCode;
            string message;

            switch (exception)
            {
                case NotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    message = exception.Message;
                    break;

                case ValidationException validationEx:
                    statusCode = StatusCodes.Status400BadRequest;
                    message = string.Join(", ", validationEx.Errors.Select(e => e.ErrorMessage));
                    break;

                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    message = "Terjadi kesalahan pada server.";
                    break;
            }

            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                error = message,
                status = statusCode
            });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(result);
        }
    }
}

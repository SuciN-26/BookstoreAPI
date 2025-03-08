using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO;
using System.Text;
using BookstoreInventory.Utils;

public class ErrorHandlerMiddlewareTests
{
    private readonly Mock<RequestDelegate> _mockNext;
    private readonly Mock<ILogger<ErrorHandlerMiddleware>> _mockLogger;

    public ErrorHandlerMiddlewareTests()
    {
        _mockNext = new Mock<RequestDelegate>();
        _mockLogger = new Mock<ILogger<ErrorHandlerMiddleware>>();
    }

    [Fact]
    public async Task Middleware_ShouldReturn500_WhenUnhandledExceptionOccurs()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var middleware = new ErrorHandlerMiddleware(async (innerHttpContext) =>
        {
            throw new Exception("Unhandled error");
        }, _mockLogger.Object);

        // Simulasi response agar bisa dituliskan oleh middleware
        context.Response.Body = new MemoryStream();

        // Act
        await middleware.Invoke(context);

        // Assert
        Assert.Equal(500, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        Assert.Contains("Terjadi kesalahan pada server.", responseBody);
    }

    [Fact]
    public async Task Middleware_ShouldReturn400_WhenValidationExceptionOccurs()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var middleware = new ErrorHandlerMiddleware(async (innerHttpContext) =>
        {
            throw new FluentValidation.ValidationException("Validation failed");
        }, _mockLogger.Object);

        context.Response.Body = new MemoryStream();

        // Act
        await middleware.Invoke(context);

        // Assert
        Assert.Equal(400, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        Assert.Contains("Validation failed", responseBody);
    }
}

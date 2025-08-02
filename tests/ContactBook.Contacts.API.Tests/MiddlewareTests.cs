using ContactBook.Contacts.API.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;

namespace ContactBook.Contacts.API.Tests.Middleware;

public class ErrorHandlingMiddlewareTests
{
    private readonly Mock<ILogger<ErrorHandlingMiddleware>> _mockLogger;
    private readonly ErrorHandlingMiddleware _middleware;

    public ErrorHandlingMiddlewareTests()
    {
        _mockLogger = new Mock<ILogger<ErrorHandlingMiddleware>>();
        _middleware = new ErrorHandlingMiddleware(
            context => throw new Exception("Test exception"),
            _mockLogger.Object);
    }

    [Fact]
    public async Task InvokeAsync_HandlesException_AndReturns500()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(500);
        context.Response.ContentType.Should().Be("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();
        
        var errorResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);
        errorResponse.GetProperty("status").GetInt32().Should().Be(500);
        errorResponse.GetProperty("message").GetString().Should().Be("An internal server error occurred. Please try again later.");
    }

    [Fact]
    public async Task InvokeAsync_LetsCancellationExceptionPropagate()
    {
        // Arrange
        var middleware = new ErrorHandlingMiddleware(
            context => throw new OperationCanceledException(),
            _mockLogger.Object);
        
        var context = new DefaultHttpContext();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => middleware.InvokeAsync(context));
    }

    [Fact]
    public async Task InvokeAsync_DoesNotHandleException_WhenResponseAlreadyStarted()
    {
        // Arrange
        var middleware = new ErrorHandlingMiddleware(
            async context =>
            {
                await context.Response.WriteAsync("Response started");
                throw new Exception("Test exception");
            },
            _mockLogger.Object);
        
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(200); // Default status, not changed to 500
    }
}

public class RequestCancellationMiddlewareTests
{
    private readonly Mock<ILogger<RequestCancellationMiddleware>> _mockLogger;

    public RequestCancellationMiddlewareTests()
    {
        _mockLogger = new Mock<ILogger<RequestCancellationMiddleware>>();
    }

    [Fact]
    public async Task InvokeAsync_HandlesCancellation_AndReturns499()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        var middleware = new RequestCancellationMiddleware(
            context => throw new OperationCanceledException(cancellationTokenSource.Token),
            _mockLogger.Object);

        var context = new DefaultHttpContext();
        context.RequestAborted = cancellationTokenSource.Token;
        context.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(499);
        
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();
        responseBody.Should().Be("Request cancelled by client");
    }

    [Fact]
    public async Task InvokeAsync_DoesNotHandleCancellation_WhenResponseAlreadyStarted()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        var middleware = new RequestCancellationMiddleware(
            async context =>
            {
                await context.Response.WriteAsync("Response started");
                throw new OperationCanceledException(cancellationTokenSource.Token);
            },
            _mockLogger.Object);

        var context = new DefaultHttpContext();
        context.RequestAborted = cancellationTokenSource.Token;
        context.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(200); // Default status, not changed to 499
    }

    [Fact]
    public async Task InvokeAsync_CallsNext_WhenNoException()
    {
        // Arrange
        var nextCalled = false;
        var middleware = new RequestCancellationMiddleware(
            context =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            },
            _mockLogger.Object);

        var context = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        nextCalled.Should().BeTrue();
    }
}
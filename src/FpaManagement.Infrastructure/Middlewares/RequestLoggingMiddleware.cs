using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FpaManagement.Infrastructure.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        // Log request
        await LogRequest(context);

        // Capture response body
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);

            // Log response
            await LogResponse(context, stopwatch.ElapsedMilliseconds);

            // Copy response back to original stream
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private async Task LogRequest(HttpContext context)
    {
        context.Request.EnableBuffering();

        var requestBody = await ReadRequestBody(context.Request);

        _logger.LogInformation(
            "HTTP Request - {Method} {Path} - User: {User} - IP: {IP} - Body: {Body}",
            context.Request.Method,
            context.Request.Path,
            context.User.Identity?.Name ?? "Anonymous",
            context.Connection.RemoteIpAddress,
            requestBody);

        context.Request.Body.Position = 0;
    }

    private async Task LogResponse(HttpContext context, long elapsedMilliseconds)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        _logger.LogInformation(
            "HTTP Response - {Method} {Path} - Status: {StatusCode} - Elapsed: {Elapsed}ms - Body: {Body}",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            elapsedMilliseconds,
            responseBody);
    }

    private async Task<string> ReadRequestBody(HttpRequest request)
    {
        if (request.Body == null || !request.Body.CanRead)
        {
            return string.Empty;
        }

        if (request.ContentLength > 0 && request.ContentLength < 2048) // Limit to 2KB
        {
            using var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true);
            return await reader.ReadToEndAsync();
        }

        return "[Body too large]";
    }
}

using System.Diagnostics;
using Serilog.Context;

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
        var correlationId = context.TraceIdentifier;

        var stopwatch = Stopwatch.StartNew();

        var request = context.Request;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            _logger.LogInformation("HTTP Request started: {Method} {Path}", request.Method, request.Path);

            try
            {
                await _next(context);
                stopwatch.Stop();

                _logger.LogInformation(
                    "HTTP Request finished: {Method} {Path} responded {StatusCode} in {Elapsed} ms",
                    request.Method,
                    request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(
                    ex,
                    "HTTP Request failed: {Method} {Path} in {Elapsed} ms",
                    request.Method,
                    request.Path,
                    stopwatch.ElapsedMilliseconds);

                throw;
            }
        }
    }
}
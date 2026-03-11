using System.Diagnostics;

namespace ApiGateway.YarpGateway.Middleware
{
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
            var requestId = Guid.NewGuid().ToString();

            context.Response.Headers["X-Request-ID"] = requestId;

            _logger.LogInformation(
                "Request {RequestId}: {Method} {Path} started. User: {User}, IP: {IP}",
                requestId,
                context.Request.Method,
                context.Request.Path,
                context.User.Identity?.Name ?? "anonymous",
                context.Connection.RemoteIpAddress);

            try
            {
                await _next(context);
                stopwatch.Stop();

                _logger.LogInformation(
                    "Request {RequestId}: Completed with status {StatusCode} in {ElapsedMs}ms",
                    requestId,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex,
                    "Request {RequestId}: Failed with error after {ElapsedMs}ms",
                    requestId,
                    stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
using System.Diagnostics;

namespace HotelManagementSystem.Helper.RequestLoggingMiddleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        // RequestDelegate and Singleton services are injected via Constructor
        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // InvokeAsync is called automatically per HTTP request
        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            // 1. Pre-Processing (On the way in)
            var method = context.Request.Method;
            var path = context.Request.Path;
            _logger.LogInformation("--> [Request] {Method} {Path}", method, path);

            // 2. Delegate to the next middleware in the pipeline
            await _next(context);

            // 3. Post-Processing (On the way out)
            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;
            _logger.LogInformation("<-- [Response] {Method} {Path} returned {StatusCode} in {Elapsed}ms",
                method, path, statusCode, stopwatch.ElapsedMilliseconds);
        }
    }
}

using HotelManagementSystem.Helper.RequestLoggingMiddleware;

namespace HotelManagementSystem.Helper.RequestLoggingMiddleware
{
 
    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}

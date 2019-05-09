using Basic.Lib.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Basic.Lib.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorLog(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorLogMiddleware>();
        }
    }
}

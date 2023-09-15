using Distributed.Core.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Distributed.Core.Extensions;
public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseJsonExceptionHandler(this IApplicationBuilder app) =>
        app.UseExceptionHandler(errorApp =>
            errorApp.UseMiddleware<JsonExceptionMiddleware>()
        );
}
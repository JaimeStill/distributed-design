using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using static System.Net.Mime.MediaTypeNames;

namespace Distributed.Core.Middleware;
public class JsonExceptionMiddleware
{
    readonly RequestDelegate next;

    public JsonExceptionMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        IExceptionHandlerFeature? error = context.Features.Get<IExceptionHandlerFeature>();

        if (error is not null)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = Application.Json;

            JsonExceptionData response = new(context, error);
            await context.Response.WriteAsJsonAsync(response);
        }

        await next(context);
    }
}
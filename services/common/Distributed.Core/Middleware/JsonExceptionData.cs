using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Distributed.Core.Middleware;
public class JsonExceptionData
{
    public string ContentType { get; private set; }
    public string Endpoint { get; private set; }
    public string Error { get; private set; }
    public string ErrorPath { get; private set; }
    public string LocalIp { get; private set; }
    public string LocalPort { get; private set; }
    public string RemoteIp { get; private set; }
    public string RemotePort { get; private set; }
    public string Source { get; private set; }
    public string Type { get; private set; }
    public string Url { get; private set; }
    public string User { get; private set; }

    public JsonExceptionData(HttpContext context, IExceptionHandlerFeature error)
    {
        ContentType = context.Request.ContentType ?? "N/A";
        Endpoint = error.Endpoint?.DisplayName ?? "N/A";
        Error = error.Error.Message ?? "An exception occurred processing the request";
        ErrorPath = error.Path ?? "N/A";
        LocalIp = context.Connection.LocalIpAddress?.ToString() ?? "N/A";
        LocalPort = context.Connection.LocalPort.ToString() ?? "N/A";
        RemoteIp = context.Connection.RemoteIpAddress?.ToString() ?? "N/A";
        RemotePort = context.Connection.RemotePort.ToString() ?? "N/A";
        Source = error.Error.Source ?? "N/A";
        Type = error.Error.GetType().ToString() ?? "N/A";
        Url = context.Request.GetDisplayUrl();
        User = context.User.Identity?.Name ?? "N/A";
    }
}
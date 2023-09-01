using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Distributed.Core.Extensions;
public static class ConfigurationExtensions
{
    public static JsonSerializerOptions ConfigureJsonOptions(JsonSerializerOptions options)
    {
        options.Converters.Add(new JsonStringEnumConverter());
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.ReferenceHandler = ReferenceHandler.IgnoreCycles;

        return options;
    }

    public static Action<JsonOptions> HttpJsonOptions => (JsonOptions options) =>
        ConfigureJsonOptions(options.JsonSerializerOptions);

    public static Action<JsonHubProtocolOptions> SignalRJsonOptions => (JsonHubProtocolOptions options) =>
        ConfigureJsonOptions(options.PayloadSerializerOptions);

    public static IServiceCollection ConfigureCorsService(this WebApplicationBuilder builder) =>
        builder
            .Services
            .AddCors(options =>
                options.AddDefaultPolicy(policy =>
                    policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                )
            );

    public static IServiceCollection ConfigureDbContext<Db>(this IServiceCollection services, IConfiguration config, string connection) where Db : DbContext =>
        services
            .AddDbContext<Db>(options =>
            {
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                options.UseSqlServer(config.GetConnectionString(connection));
            });

    public static IMvcBuilder ConfigureApiServices(this WebApplicationBuilder builder) =>
        builder
            .Services
            .AddControllers()
            .AddJsonOptions(HttpJsonOptions);

    public static ISignalRServerBuilder ConfigureSignalRServices(this WebApplicationBuilder builder) =>
        builder
            .Services
            .AddSignalR()
            .AddJsonProtocol(SignalRJsonOptions);

    public static HubConnectionBuilder ConfigureJsonFormat(this HubConnectionBuilder builder) =>
        builder.AddJsonProtocol(SignalRJsonOptions);
}
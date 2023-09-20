using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Distributed.Core.Gateway;
using Distributed.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Distributed.Core.Extensions;
public static class ServiceExtensions
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
                    policy
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .WithOrigins(
                            builder
                                .Configuration
                                .GetConfigArray("CorsOrigins")
                        )
                        .WithExposedHeaders(
                            "Access-Control-Allow-Origin",
                            "Access-Control-Allow-Credentials"
                        )
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

    public static string GetEventEndpoint(IConfiguration config, string service) =>
        config.GetRequiredSection("Events")
              .GetValue<string>(service)
        ?? throw new Exception($"Event Configuration: The requested Event service {service} has not been configured");

    public static HubConnectionBuilder ConfigureJsonFormat(this HubConnectionBuilder builder) =>
        builder.AddJsonProtocol(SignalRJsonOptions);

    public static void ConfigureGatewayOptions(this WebApplicationBuilder builder) =>
        builder
            .Services
            .Configure<GatewayOptions>(
                builder.Configuration.GetSection(GatewayOptions.Gateway)
            );

    public static void AddGatewayService(this IServiceCollection services) =>
        services.AddSingleton<GatewayService>();

    public static void AddAppServices(this IServiceCollection services)
    {
        Assembly? entry = Assembly.GetEntryAssembly();

        if (entry is not null)
        {
            IEnumerable<Assembly> assemblies = entry
                .GetReferencedAssemblies()
                .Select(Assembly.Load)
                .Append(entry)
                .Where(x =>
                    x.GetTypes()
                        .Any(IsValidServiceRegistrant)
                );

            IEnumerable<Type>? registrants = assemblies
                .SelectMany(x =>
                    x.GetTypes()
                        .Where(IsValidServiceRegistrant)
                );

            if (registrants is not null)
                foreach (Type registrant in registrants)
                    ((ServiceRegistrant?)Activator.CreateInstance(registrant, services))?.Register();
        }
    }

    static string[] GetConfigArray(this IConfiguration config, string section) =>
        config
            .GetSection(section)
            .Get<string[]>()
        ?? Array.Empty<string>();

    static bool IsValidServiceRegistrant(this Type t) =>
        t.IsClass
        && !t.IsAbstract
        && t.IsSubclassOf(typeof(ServiceRegistrant));
}
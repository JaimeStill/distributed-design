using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Distributed.Core.Gateway;
public class GatewayService
{
    readonly Gateway gateway;

    public GatewayService(IConfiguration config)
    {
        gateway = config
            .GetSection("Gateway")
            .Get<Gateway>()
        ?? throw new Exception(
            $"Invalid Gateway Configuration: configuration is missing or invalid. It must be provided when registering a Gateway service."
        );
    }

    public GatewayService(Gateway gateway)
    {
        this.gateway = gateway;
    }

    public Guid GatewayId => gateway.Id;
    public List<Endpoint> Endpoints => gateway.Endpoints;

    public Endpoint GetEndpoint(string name) =>
        gateway.Endpoints.First(x =>
            x.Name.ToLower() == name.ToLower()
        );
}

public static class GatewayRegistration
{
    public static void AddGatewayService(this IServiceCollection services) =>
        services.AddSingleton<GatewayService>();
}
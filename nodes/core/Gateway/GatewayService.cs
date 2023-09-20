using Microsoft.Extensions.Options;

namespace Distributed.Core.Gateway;
public class GatewayService
{
    readonly IOptionsMonitor<GatewayOptions> gateway;

    public GatewayService(IOptionsMonitor<GatewayOptions> gateway)
    {
        this.gateway = gateway;
    }

    public Guid GatewayId => gateway.CurrentValue.Id;
    public List<Endpoint> Endpoints => gateway.CurrentValue.Endpoints;

    public Endpoint GetEndpoint(string name)
    {
        Endpoint result = gateway.CurrentValue.Endpoints.First(x =>
            x.Name.ToLower() == name.ToLower()
        );

        return result;
    }
}
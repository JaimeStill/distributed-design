using Distributed.Contracts.Gateways;
using Distributed.Core.Services;

namespace Distributed.Proposals.Api;
public class Registrant : ServiceRegistrant
{
    public Registrant(IServiceCollection services) : base(services)
    { }

    public override void Register()
    {
        services.AddSingleton<WorkflowsGateway>();
    }
}
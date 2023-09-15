using Distributed.Core.Services;
using Workflows.Contracts;

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
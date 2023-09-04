using Distributed.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Distributed.Proposals.Logic;
public class Registrant : ServiceRegistrant
{
    public Registrant(IServiceCollection services) : base(services)
    { }

    public override void Register()
    {
        services.AddScoped<ProposalQuery>();
        services.AddScoped<ProposalCommand>();
    }
}
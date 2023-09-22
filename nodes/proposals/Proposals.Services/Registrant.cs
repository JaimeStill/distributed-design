using Distributed.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Proposals.Services;
public class Registrant : ServiceRegistrant
{
    public Registrant(IServiceCollection services) : base(services)
    { }

    public override void Register()
    {
        services.AddScoped<ProposalQuery>();
        services.AddScoped<ProposalCommand>();
        services.AddScoped<PackageSaga>();
    }
}
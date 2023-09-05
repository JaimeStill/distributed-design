using Distributed.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Distributed.Workflows.Logic;
public class Registrant : ServiceRegistrant
{
    public Registrant(IServiceCollection services) : base(services)
    { }

    public override void Register()
    {
        services.AddScoped<PackageQuery>();
        services.AddScoped<PackageCommand>();
    }
}
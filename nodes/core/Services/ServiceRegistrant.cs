using Microsoft.Extensions.DependencyInjection;

namespace Distributed.Core.Services;
public abstract class ServiceRegistrant
{
    protected readonly IServiceCollection services;

    public ServiceRegistrant(IServiceCollection services)
    {
        this.services = services;
    }

    public abstract void Register();
}
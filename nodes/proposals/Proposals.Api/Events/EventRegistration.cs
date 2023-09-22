using Distributed.Core.Services;
using Proposals.Services;
using Workflows.Contracts;

namespace Proposals.Api.Events;
public static class EventRegistration
{
    public static void RegisterEventListeners(this IServiceCollection services)
    {
        services.AddEventClient<PackageEventListener, Package>();
        services.AddHostedService<PackageEventStartup>();
    }
}
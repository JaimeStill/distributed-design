using Microsoft.Extensions.DependencyInjection;

namespace Distributed.Core.Services;
public static class EventClientRegistration
{
    public static void AddEventClient<C,T>(this IServiceCollection services)
    where C : EventClient<T> =>
        services.AddSingleton<C>();
}
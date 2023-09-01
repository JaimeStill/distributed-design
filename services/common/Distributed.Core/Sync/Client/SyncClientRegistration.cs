using Microsoft.Extensions.DependencyInjection;

namespace Distributed.Core.Sync.Client;
public static class SyncClientRegistration
{
    public static void AddSyncClient<C,T>(this IServiceCollection services)
    where C : SyncClient<T> =>
        services.AddSingleton<C>();
}
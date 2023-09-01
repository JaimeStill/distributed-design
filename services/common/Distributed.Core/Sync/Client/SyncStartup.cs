using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Distributed.Core.Sync.Client;
public abstract class SyncStartup<C,T> : BackgroundService
where C : SyncClient<T>
{
    private readonly IServiceProvider provider;

    public SyncStartup(IServiceProvider provider)
    {
        this.provider = provider;
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        try
        {
            using IServiceScope scope = provider.CreateScope();

            C client = scope.ServiceProvider.GetRequiredService<C>();

            await client.Connect(token);
        }
        catch when (token.IsCancellationRequested)
        {
            return;
        }
    }
}
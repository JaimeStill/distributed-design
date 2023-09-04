using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Distributed.Core.Services;
public abstract class EventStartup<C,T> : BackgroundService
where C : EventClient<T>
{
    private readonly IServiceProvider provider;

    public EventStartup(IServiceProvider provider)
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
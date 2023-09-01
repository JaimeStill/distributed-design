using Microsoft.AspNetCore.SignalR;

namespace Distributed.Core.Sync.Hub;
public abstract class SyncHub<T, H> : Hub<H>
where H : class, ISyncHub<T>
{
    public void LogAction(ISyncMessage<T> message, string action) =>
        Console.WriteLine($"{action} message received: {message.Message}");

    public async Task Ping()
    {
        Console.WriteLine("Ping received");

        await Clients
            .All
            .Ping();
    }
}
using Microsoft.AspNetCore.SignalR;

namespace Distributed.Core.Services;
public abstract class EventHub<T,THub> : Hub<THub>
where THub : class, IEventHub<T>
{
    public void LogAction(IEventMessage<T> message, string action) =>
        Console.WriteLine($"{action} message received: {message.Message}");

    public async Task Ping()
    {
        Console.WriteLine("Ping received");

        await Clients
            .All
            .Ping();
    }
}
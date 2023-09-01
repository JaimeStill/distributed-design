using Distributed.Core.Hubs;
using Distributed.Core.Schema;
using Distributed.Core.Sync;
using Microsoft.AspNetCore.SignalR;

namespace Distributed.Core.Services;
public abstract class EntityEvent<T,H> : IEvent<T>
where T : Entity
where H : EntityEventHub<T>
{
    protected IHubContext<H,IEventHub<T>> sync;

    public EntityEvent(IHubContext<H,IEventHub<T>> sync)
    {
        this.sync = sync;
    }
        
    public async Task Add(T entity)
    {
        SyncMessage<T> message = GenerateMessage(entity, "created");

        await sync
            .Clients
            .All
            .Add(message);
    }

    public async Task Update(T entity)
    {
        SyncMessage<T> message = GenerateMessage(entity, "updated");

        await sync
            .Clients
            .All
            .Update(message);
    }

    public async Task Remove(T entity)
    {
        SyncMessage<T> message = GenerateMessage(entity, "removed");

        await sync
            .Clients
            .All
            .Remove(message);
    }

    protected SyncMessage<T> GenerateMessage(T entity, string action)
    {
        SyncMessage<T> message = new(
            entity,
            SetMessage(entity, action)
        );

        LogAction(message, action);

        return message;
    }

    protected virtual string SetMessage(T entity, string action) =>
        $"{typeof(T)} {entity.Value} successfully {action}";

    protected virtual void LogAction(ISyncMessage<T> message, string action) =>
        Console.WriteLine($"{action}: {message.Message}");
}
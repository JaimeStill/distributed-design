using Distributed.Core.Schema;
using Distributed.Core.Sync;
using Distributed.Core.Sync.Client;
using Microsoft.EntityFrameworkCore;

namespace Distributed.Core.Services;
public abstract class EntityEventListener<T,S,Db> : SyncClient<T>, IEventListener<T>
where T : Entity
where S : EntitySaga<T,Db>
where Db : DbContext
{
    protected readonly S saga;

    public SyncAction Add { get; protected set; }
    public SyncAction Update { get; protected set; }
    public SyncAction Remove { get; protected set; }

    public Task HandleAdd(ISyncMessage<T> message) =>
        saga.OnAdd(message.Data);

    public Task HandleUpdate(ISyncMessage<T> message) =>
        saga.OnUpdate(message.Data);

    public Task HandleRemove(ISyncMessage<T> message) =>
        saga.OnRemove(message.Data);

    public EntityEventListener(S saga, string endpoint) : base(endpoint)
    {
        this.saga = saga;

        Add = new("Add", connection);
        Update = new("Update", connection);
        Remove = new("Remove", connection);

        Add.Set<ISyncMessage<T>>(HandleAdd);
        Update.Set<ISyncMessage<T>>(HandleUpdate);
        Remove.Set<ISyncMessage<T>>(HandleRemove);
    }
}
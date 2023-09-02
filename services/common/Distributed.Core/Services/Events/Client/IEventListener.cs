using Distributed.Core.Schema;
using Distributed.Core.Sync;
using Distributed.Core.Sync.Client;

namespace Distributed.Core.Services;
public interface IEventListener<T> : ISyncClient<T>
where T : Entity
{
    SyncAction Add { get; }
    SyncAction Update { get; }
    SyncAction Remove { get; }

    Task HandleAdd(ISyncMessage<T> message);
    Task HandleUpdate(ISyncMessage<T> message);
    Task HandleRemove(ISyncMessage<T> message);
}
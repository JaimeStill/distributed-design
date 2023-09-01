using Distributed.Core.Schema;
using Distributed.Core.Sync;
using Distributed.Core.Sync.Hub;

namespace Distributed.Core.Hubs;
public interface IEventHub<T> : ISyncHub<T>
where T : Entity
{
    Task Add(ISyncMessage<T> message);
    Task Update(ISyncMessage<T> message);
    Task Remove(ISyncMessage<T> message);
}
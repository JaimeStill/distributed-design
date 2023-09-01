using Distributed.Core.Schema;
using Distributed.Core.Sync.Hub;

namespace Distributed.Core.Hubs;
public abstract class EntityEventHub<T> : SyncHub<T, IEventHub<T>>
where T : Entity
{ }
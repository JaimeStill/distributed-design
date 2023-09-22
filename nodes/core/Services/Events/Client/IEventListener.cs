using Distributed.Core.Entities;

namespace Distributed.Core.Services;
public interface IEventListener<T> : IEventClient<T>
where T : Entity
{
    EventAction OnAdd { get; }
    EventAction OnUpdate { get; }
    EventAction OnRemove { get; }
}
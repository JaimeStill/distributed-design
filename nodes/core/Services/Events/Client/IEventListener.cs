using Distributed.Core.Entities;

namespace Distributed.Core.Services;
public interface IEventListener<T> : IEventClient<T>
where T : Entity
{
    EventAction Add { get; }
    EventAction Update { get; }
    EventAction Remove { get; }
}
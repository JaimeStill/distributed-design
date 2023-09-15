using Distributed.Core.Entities;

namespace Distributed.Core.Services;
public abstract class EventListener<T,S> : EventClient<T>, IEventListener<T>
where T : Entity
where S : ISaga<T>
{
    protected readonly S saga;

    public EventAction Add { get; }
    public EventAction Update { get; }
    public EventAction Remove { get; }

    public EventListener(S saga, string endpoint) : base(endpoint)
    {
        this.saga = saga;

        Add = new("Add", connection);
        Update = new("Update", connection);
        Remove = new("Remove", connection);
    }
}
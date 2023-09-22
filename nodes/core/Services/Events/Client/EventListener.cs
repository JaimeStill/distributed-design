using Distributed.Core.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Distributed.Core.Services;
public abstract class EventListener<T,S> : EventClient<T>, IEventListener<T>
where T : Entity
where S : ISaga<T>
{
    protected readonly IServiceProvider provider;

    public EventAction OnAdd { get; }
    public EventAction OnUpdate { get; }
    public EventAction OnRemove { get; }

    public EventListener(IServiceProvider provider, string endpoint) : base(endpoint)
    {
        this.provider = provider;

        OnAdd = new("OnAdd", connection);
        OnUpdate = new("OnUpdate", connection);
        OnRemove = new("OnRemove", connection);
    }

    protected Func<EventMessage<T>, Task> HandleEvent(Func<EventMessage<T>, S, Task> action) =>
        async (EventMessage<T> message) =>
        {
            using IServiceScope scope = provider.CreateScope();
            S saga = scope.ServiceProvider.GetRequiredService<S>();
            await action(message, saga);
        };

    protected override void DisposeEvents()
    {
        base.DisposeEvents();
        OnAdd.Dispose();
        OnUpdate.Dispose();
        OnRemove.Dispose();
    }
}
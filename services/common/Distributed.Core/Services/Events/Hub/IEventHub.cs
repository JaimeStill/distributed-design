namespace Distributed.Core.Services;
public interface IEventHub<T>
{
    Task Ping();
    Task Sync(IEventMessage<T> message);

    Task OnAdd(IEventMessage<T> message);
    Task OnUpdate(IEventMessage<T> message);
    Task OnRemove(IEventMessage<T> message);
}
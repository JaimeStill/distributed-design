namespace Distributed.Core.Services;
public interface IEventHub<T>
{
    Task Ping();
    Task Sync(IEventMessage<T> message);

    Task Add(IEventMessage<T> message);
    Task Update(IEventMessage<T> message);
    Task Remove(IEventMessage<T> message);
}
namespace Distributed.Core.Sync.Hub;
public interface ISyncHub<T>
{
    Task Ping();
    Task Sync(ISyncMessage<T> message);
}
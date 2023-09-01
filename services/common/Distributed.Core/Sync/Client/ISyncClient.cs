namespace Distributed.Core.Sync.Client;
public interface ISyncClient<T> : IAsyncDisposable
{
    SyncClientStatus Status { get; }
    SyncAction OnPing { get; }
    SyncAction OnSync { get; }
    Task Connect(CancellationToken token);
    Task Ping();
}
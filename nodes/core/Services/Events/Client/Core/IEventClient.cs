namespace Distributed.Core.Services;
public interface IEventClient<T> : IAsyncDisposable
{
    EventClientStatus Status { get; }
    EventAction OnPing { get; }
    EventAction OnSync { get; }
    Task Connect(CancellationToken token);
    Task Ping();
}
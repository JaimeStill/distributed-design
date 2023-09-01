using Microsoft.AspNetCore.SignalR.Client;

namespace Distributed.Core.Sync.Client;
public class SyncAction : IDisposable
{
    private readonly string method;
    private readonly HubConnection client;
    private IDisposable? subscription;

    public SyncAction(string method, HubConnection client)
    {
        this.method = method;
        this.client = client;
    }

    public void Set<T>(Func<T, Task> action)
    {
        subscription?.Dispose();
        subscription = client.On(method, action);
    }

    public void Set<T>(Action<T> action)
    {
        subscription?.Dispose();
        subscription = client.On(method, action);
    }

    public void Set(Action action)
    {
        subscription?.Dispose();
        subscription = client.On(method, action);
    }

    public void Dispose()
    {
        subscription?.Dispose();
        GC.SuppressFinalize(this);
    }    
}
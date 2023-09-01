namespace Distributed.Core.Sync;
public class SyncMessage<T> : ISyncMessage<T>
{
    public Guid Id { get; private set; }
    public T Data { get; set; }
    public string Message { get; set; }

    public SyncMessage(
        T data,
        string? message = null
    )
    {
        Id = Guid.NewGuid();
        Data = data;
        Message = message ?? string.Empty;
    }
}
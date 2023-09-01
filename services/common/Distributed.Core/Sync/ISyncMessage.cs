namespace Distributed.Core.Sync;
public interface ISyncMessage<T>
{
    public Guid Id { get; }
    public T Data { get; set; }
    public string Message { get; set; }
}
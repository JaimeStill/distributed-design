namespace Distributed.Core.Services;
public interface IEventMessage<T>
{
    public Guid Id { get; }
    public T Data { get; set; }
    public string Message { get; set; }
}
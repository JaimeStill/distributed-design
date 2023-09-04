namespace Distributed.Core.Services;
public class EventMessage<T> : IEventMessage<T>
{
    public Guid Id { get; private set; }
    public T Data { get; set; }
    public string Message { get; set; }

    public EventMessage(
        T data,
        string? message = null
    )
    {
        Id = Guid.NewGuid();
        Data = data;
        Message = message ?? string.Empty;
    }
}
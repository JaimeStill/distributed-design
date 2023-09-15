namespace Distributed.Core.Messages;
public class HookMessage<T>
{
    public T Value { get; private set; }
    public Exception? Exception { get; private set; }

    public HookMessage(T value, Exception? ex = null)
    {
        Value = value;
        Exception = ex;
    }
}
using Distributed.Core.Messages;

namespace Distributed.Core.Services;
public interface ISaga<T>
{
    Task<ApiMessage<T>> OnAdd(T entity);
    Task<ApiMessage<T>> OnUpdate(T entity);
    Task<ApiMessage<T>> OnRemove(T entity);
}
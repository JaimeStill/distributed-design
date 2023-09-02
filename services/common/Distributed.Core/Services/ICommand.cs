using Distributed.Core.Messages;
using Distributed.Core.Schema;

namespace Distributed.Core.Services;
public interface ICommand<T>
where T : Entity
{
    Task<bool> ValidateValue(T entity);
    Task<ValidationMessage> Validate(T entity);
    Task<ApiMessage<T>> Save(T entity);
    Task<ApiMessage<int>> Remove(T entity);
}
using Distributed.Core.Messages;
using Distributed.Core.Schema;
using Microsoft.EntityFrameworkCore;

namespace Distributed.Core.Services;
public interface ICommand<T,Db>
where T : Entity
where Db : DbContext
{
    Task<bool> ValidateValue(T entity);
    Task<ValidationMessage> Validate(T entity);
    Task<ApiMessage<T>> Save(T entity);
    Task<ApiMessage<int>> Remove(T entity);
}
using Distributed.Core.Schema;

namespace Distributed.Core.Services;
public interface IEvent<T>
where T : Entity
{
    Task Add(T entity);
    Task Update(T entity);
    Task Remove(T entity);
}
using Distributed.Core.Entities;

namespace Distributed.Core.Services;
public interface IQuery<T>
where T : Entity
{
    Task<List<T>> Get();
    Task<T?> GetById(int id);
}
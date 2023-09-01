using Distributed.Core.Schema;
using Microsoft.EntityFrameworkCore;

namespace Distributed.Core.Services;
public interface IQuery<T,Db>
where T : Entity
where Db : DbContext
{
    Task<List<T>> Get();
    Task<T?> GetById(int id);
}
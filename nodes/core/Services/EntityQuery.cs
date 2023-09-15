using Distributed.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Distributed.Core.Services;
public abstract class EntityQuery<T,Db> : IQuery<T>
where T : Entity
where Db : DbContext
{
    protected Db db;
    protected DbSet<T> Set => db.Set<T>();

    public EntityQuery(Db db)
    {
        this.db = db;
    }

    public virtual async Task<List<T>> Get() =>
        await Set.ToListAsync();

    public virtual async Task<T?> GetById(int id) =>
        await Set.FindAsync(id);
}
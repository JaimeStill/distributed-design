using Distributed.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Distributed.Core.Services;
public abstract class EntitySaga<T,Db> : ISaga<T>
where T : Entity
where Db : DbContext
{
    protected readonly Db db;
    public EntitySaga(Db db)
    {
        this.db = db;
    }
}
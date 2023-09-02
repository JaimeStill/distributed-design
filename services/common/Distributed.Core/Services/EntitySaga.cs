using Distributed.Core.Messages;
using Distributed.Core.Schema;
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

    public abstract Task<ApiMessage<T>> OnAdd(T entity);
    public abstract Task<ApiMessage<T>> OnUpdate(T entity);
    public abstract Task<ApiMessage<T>> OnRemove(T entity);
}
using Distributed.Core.Messages;
using Distributed.Core.Schema;
using Microsoft.EntityFrameworkCore;

namespace Distributed.Core.Services;
public abstract class EntityCommand<T,Db> : ICommand<T,Db>
where T : Entity
where Db : DbContext
{
    protected Db db;
    protected DbSet<T> Set => db.Set<T>();

    public EntityCommand(Db db)
    {
        this.db = db;
    }

    protected virtual Func<T, Task<T>>? OnAdd { get; set; }
    protected virtual Func<T, Task<T>>? OnUpdate { get; set; }
    protected virtual Func<T, Task<T>>? OnSave { get; set; }
    protected virtual Func<T, Task<T>>? OnRemove { get; set; }

    protected virtual Func<T, Task>? AfterAdd { get; set; }
    protected virtual Func<T, Task>? AfterUpdate { get; set; }
    protected virtual Func<T, Task>? AfterSave { get; set; }
    protected virtual Func<T, Task>? AfterRemove { get; set; }

    #region Internal

    protected async Task<ApiMessage<T>> Add(T entity)
    {
        try
        {
            if (OnAdd is not null)
                entity = await OnAdd(entity);

            await db.Set<T>().AddAsync(entity);
            await db.SaveChangesAsync();

            if (AfterAdd is not null)
                await AfterAdd(entity);

            return new (entity, $"{typeof(T)} successfully added");
        }
        catch (Exception ex)
        {
            return new("Add", ex);
        }
    }

    protected async Task<ApiMessage<T>> Update(T entity)
    {
        try
        {
            if (OnUpdate is not null)
                entity = await OnUpdate(entity);

            db.Set<T>().Update(entity);
            await db.SaveChangesAsync();

            if (AfterUpdate is not null)
                await AfterUpdate(entity);

            return new(entity, $"{typeof(T)} successfully updated");
        }
        catch (Exception ex)
        {
            return new("Update", ex);
        }
    }

    #endregion

    #region Public

    public virtual async Task<bool> ValidateValue(T entity) =>
        !await db.Set<T>()
            .AnyAsync(x =>
                x.Id != entity.Id
                && x.Value.ToLower() == entity.Value.ToLower()
            );

    public virtual async Task<ValidationMessage> Validate(T entity)
    {
        ValidationMessage result = new();

        if (string.IsNullOrWhiteSpace(entity.Value))
            result.AddMessage("Value is required");

        if (!await ValidateValue(entity))
            result.AddMessage("Value is already in use");

        return result;
    }

    public async Task<ApiMessage<T>> Save(T entity)
    {
        ValidationMessage validity = await Validate(entity);

        if (validity.IsValid)
        {
            if (OnSave is not null)
                entity = await OnSave(entity);

            ApiMessage<T> result = entity.Id > 0
                ? await Update(entity)
                : await Add(entity);

            if (AfterSave is not null)
                await AfterSave(entity);

            return result;
        }
        else
            return new(validity);
    }

    public async Task<ApiMessage<int>> Remove(T entity)
    {
        try
        {
            if (OnRemove is not null)
                entity = await OnRemove(entity);

            db.Set<T>().Remove(entity);

            int result = await db.SaveChangesAsync();

            if (result > 0)
            {
                if (AfterRemove is not null)
                    await AfterRemove(entity);

                return new(entity.Id, $"{typeof(T)} successfully removed");
            }
            else
                return new("Remove", new Exception("The operation was not successful"));
        }
        catch (Exception ex)
        {
            return new("Remove", ex);
        }
    }

    #endregion
}
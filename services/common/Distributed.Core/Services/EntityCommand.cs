using Distributed.Core.Messages;
using Distributed.Core.Schema;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Distributed.Core.Services;
public abstract class EntityCommand<T,THub,IHub,Db> : ICommand<T>
where T : Entity
where THub : EventHub<T,IHub>
where IHub : class, IEventHub<T>
where Db : DbContext
{
    protected readonly Db db;
    protected readonly IHubContext<THub,IHub> events;
    protected DbSet<T> Set => db.Set<T>();

    public EntityCommand(Db db, IHubContext<THub,IHub> events)
    {
        this.db = db;
        this.events = events;
    }

    protected virtual Func<T, Task<T>>? OnAdd { get; set; }
    protected virtual Func<T, Task<T>>? OnUpdate { get; set; }
    protected virtual Func<T, Task<T>>? OnSave { get; set; }
    protected virtual Func<T, Task<T>>? OnRemove { get; set; }

    protected virtual Func<T, Task<T>>? AfterAdd { get; set; }
    protected virtual Func<T, Task<T>>? AfterUpdate { get; set; }
    protected virtual Func<T, Task<T>>? AfterSave { get; set; }
    protected virtual Func<T, Task>? AfterRemove { get; set; }

    Func<T, Task> SyncAdd => async (T entity) =>
    {
        EventMessage<T> message = GenerateMessage(entity, "created");

        await events
            .Clients
            .All
            .OnAdd(message);
    };

    Func<T, Task> SyncUpdate => async (T entity) =>
    {
        EventMessage<T> message = GenerateMessage(entity, "updated");

        await events
            .Clients
            .All
            .OnUpdate(message);
    };

    Func<T, Task> SyncRemove => async (T entity) =>
    {
        EventMessage<T> message = GenerateMessage(entity, "removed");

        await events
            .Clients
            .All
            .OnRemove(message);
    };

    #region Internal

    protected virtual void LogAction(IEventMessage<T> message, string action) =>
        Console.WriteLine($"{action}: {message.Message}");

    protected virtual string SetMessage(T entity, string action) =>
        $"{typeof(T)} {entity.Value} successfully {action}";

    protected EventMessage<T> GenerateMessage(T entity, string action)
    {
        EventMessage<T> message = new(
            entity,
            SetMessage(entity, action)
        );

        LogAction(message, action);

        return message;
    }

    protected async Task<ApiMessage<T>> Add(T entity)
    {
        try
        {
            if (OnAdd is not null)
                entity = await OnAdd(entity);

            await db.Set<T>().AddAsync(entity);
            await db.SaveChangesAsync();

            if (AfterAdd is not null)
                entity = await AfterAdd(entity);

            await SyncAdd(entity);

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
                entity = await AfterUpdate(entity);

            await SyncUpdate(entity);

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

                await SyncRemove(entity);

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
using Distributed.Core.Messages;
using Distributed.Core.Schema;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Distributed.Core.Services;
public abstract class EntityCommand<T, THub, IHub, Db> : ICommand<T>
where T : Entity
where THub : EventHub<T, IHub>
where IHub : class, IEventHub<T>
where Db : DbContext
{
    protected readonly Db db;
    protected readonly IHubContext<THub, IHub> events;
    protected DbSet<T> Set => db.Set<T>();

    public EntityCommand(Db db, IHubContext<THub, IHub> events)
    {
        this.db = db;
        this.events = events;
    }

    protected async Task<T> HandleHook(T entity, Func<T, Task<HookMessage<T>>> hook)
    {
        HookMessage<T> result = await hook(entity);

        if (result.Exception is not null)
            throw result.Exception;

        return result.Value;
    }

    protected virtual Func<T, Task<HookMessage<T>>>? OnAdd { get; set; }
    protected virtual Func<T, Task<HookMessage<T>>>? OnUpdate { get; set; }
    protected virtual Func<T, Task<HookMessage<T>>>? OnSave { get; set; }
    protected virtual Func<T, Task<HookMessage<T>>>? OnRemove { get; set; }

    protected virtual Func<T, Task<HookMessage<T>>>? AfterAdd { get; set; }
    protected virtual Func<T, Task<HookMessage<T>>>? AfterUpdate { get; set; }
    protected virtual Func<T, Task<HookMessage<T>>>? AfterSave { get; set; }
    protected virtual Func<T, Task<HookMessage<T>>>? AfterRemove { get; set; }

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
                entity = await HandleHook(entity, OnAdd);

            await db.Set<T>().AddAsync(entity);
            await db.SaveChangesAsync();

            if (AfterAdd is not null)
                entity = await HandleHook(entity, AfterAdd);

            await SyncAdd(entity);

            return new(entity, $"{typeof(T)} successfully added");
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
                entity = await HandleHook(entity, OnUpdate);

            db.Set<T>().Update(entity);
            await db.SaveChangesAsync();

            if (AfterUpdate is not null)
                entity = await HandleHook(entity, AfterUpdate);

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
            using IDbContextTransaction transaction = await db.Database.BeginTransactionAsync();

            try
            {
                if (OnSave is not null)
                    entity = await HandleHook(entity, OnSave);

                ApiMessage<T> result = entity.Id > 0
                    ? await Update(entity)
                    : await Add(entity);

                if (AfterSave is not null)
                    entity = await HandleHook(entity, AfterSave);

                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new("Save", ex);
            }
        }
        else
            return new(validity);
    }

    public async Task<ApiMessage<int>> Remove(T entity)
    {
        using IDbContextTransaction transaction = await db.Database.BeginTransactionAsync();

        try
        {
            if (OnRemove is not null)
                entity = await HandleHook(entity, OnRemove);

            db.Set<T>().Remove(entity);

            int result = await db.SaveChangesAsync();

            if (result > 0)
            {
                if (AfterRemove is not null)
                    entity = await HandleHook(entity, AfterRemove);

                await SyncRemove(entity);

                return new(entity.Id, $"{typeof(T)} successfully removed");
            }
            else
            {
                await transaction.RollbackAsync();
                return new("Remove", new Exception("The operation was not successful"));
            }
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return new("Remove", ex);
        }
    }

    #endregion
}
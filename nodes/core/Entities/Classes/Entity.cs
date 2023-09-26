using Microsoft.EntityFrameworkCore;

namespace Distributed.Core.Entities;

public abstract class Entity
{
    public int Id { get; set; }
    public string Type { get; set; }
    public virtual string Value { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public DateTime DateModified { get; set; } = DateTime.UtcNow;

    public Entity()
    {
        Type = GetType().FullName
            ?? "Distributed.Core.Entities.Entity";
    }

    public virtual void OnSaving(EntityState state)
    {
        DateModified = DateTime.UtcNow;

        if (state == EntityState.Added || Id < 1)
            DateCreated = DateModified;
    }
}
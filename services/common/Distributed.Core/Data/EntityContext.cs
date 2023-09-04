using System.Reflection;
using Distributed.Core.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Distributed.Core.Data;
public abstract class EntityContext<T> : DbContext where T : DbContext
{
    public EntityContext(DbContextOptions<T> options) : base(options)
    {
        SavingChanges += OnSaving;
    }

    protected IEnumerable<EntityEntry> ChangeTrackerEntities() =>
        ChangeTracker
            .Entries()
            .Where(x => x.Entity is Entity);

    protected virtual void OnSaving(object? sender, SavingChangesEventArgs e)
    {
        IEnumerable<EntityEntry> entries = ChangeTrackerEntities();

        foreach (EntityEntry entry in entries)
            ((Entity)entry.Entity).OnSaving(entry.State);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<Enum>()
            .HaveConversion<string>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly()
        );
    }
}
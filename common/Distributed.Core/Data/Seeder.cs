using System.Reflection;
using Distributed.Core.Extensions;
using Distributed.Core.Schema;
using Microsoft.EntityFrameworkCore;

namespace Distributed.Core.Data;
public abstract class Seeder<E, C> where E : Entity where C : DbContext
{
    protected readonly C db;

    public Seeder(C db)
    {
        this.db = db;
    }

    protected abstract Task<List<E>> Generate();

    public virtual async Task<List<E>> Seed()
    {
        Console.WriteLine($"Initializing {typeof(E).Name} Records");

        if (await db.Set<E>().AnyAsync())
            return await db.Set<E>().ToListAsync();
        else
            return await Generate();
    }
}

public static class SeederExtensions
{
    public static async Task Seed<C>(this C db) where C : DbContext
    {
        Console.WriteLine($"Seeding into {db.Database.GetConnectionString()}");

        IEnumerable<Type>? seeders = Assembly
            .GetEntryAssembly()
            ?.GetTypes()
            .Where(x =>
                x.IsClass
                && !x.IsAbstract
                && x.BaseType != null
                && x.BaseType.Name.Contains("Seeder")
                && x.GetMethod("Seed") != null
            );

        if (seeders is not null)
        {
            foreach (Type seeder in seeders)
            {
                object? s = Activator.CreateInstance(seeder, db);

                if (s is not null)
                {
                    MethodInfo? seed = s
                        .GetType()
                        .GetMethod("Seed");

                    if (seed is not null)
                        await seed.InvokeAsync(s);
                }
            }
        }
    }
}
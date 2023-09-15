using System.Reflection;
using Distributed.Core.Extensions;
using Distributed.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Distributed.Core.Data;
public abstract class Seeder<E, C> where E : Entity where C : DbContext
{
    protected readonly C db;
    protected abstract List<E> Required { get; }
    protected abstract List<E>? Optional { get; }

    public Seeder(C db)
    {
        this.db = db;
    }

    protected abstract Task<List<E>> GenerateRequired();
    protected abstract Task<List<E>> GenerateOptional();

    public virtual async Task<List<E>> Seed()
    {
        List<E> results = new();

        Console.WriteLine($"Initializing {typeof(E).Name} Records");

        if (Optional is not null && !await db.Set<E>().AnyAsync())
            results.AddRange(await GenerateOptional());

        results.AddRange(await GenerateRequired());
        
        return results;
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
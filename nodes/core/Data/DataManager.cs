using Microsoft.EntityFrameworkCore;

namespace Distributed.Core.Data;
public class DataManager<T> : IAsyncDisposable where T : DbContext
{
    readonly bool destroy;
    public T? Context { get; private set; }
    public string? Connection => Context?.Database.GetConnectionString();

    public DataManager(
        string connection,
        bool destroy = false,
        bool unique = false
    )
    {
        this.destroy = destroy;

        if (unique)
            connection = $"{connection}-{Guid.NewGuid()}";

        Context = ContextBuilder<T>.Build(connection);
    }

    public Task<bool> Destroy() => Context!.Database.EnsureDeletedAsync();

    public async Task<bool> Initialize()
    {
        if (Context is null)
            return false;

        if (destroy)
            await Destroy();

        await Context.Database.MigrateAsync();

        return true;
    }

    public async Task Execute()
    {
        if (Context is not null)
        {
            Console.WriteLine("Initializing database and applying migrations");
            await Initialize();
            await Context.Seed();
        }
        else Console.WriteLine("DataManager does not have an initialized DbContext");
    }

    public async ValueTask DisposeAsync()
    {
        if (destroy)
            await Destroy();

        Context?.Dispose();

        GC.SuppressFinalize(this);
    }
}
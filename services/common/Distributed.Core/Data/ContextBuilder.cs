using Microsoft.EntityFrameworkCore;

namespace Distributed.Core.Data;
public class ContextBuilder<T> where T : DbContext
{
    private ContextBuilder() { }

    public static T Build(string connectionString) =>
        GetDbContext(connectionString, () =>
            new DbContextOptionsBuilder<T>()
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
        );

    public static T Build(string connectionString, Func<DbContextOptionsBuilder<T>> config) =>
        GetDbContext(connectionString, config);

    static T GetDbContext(string connectionString, Func<DbContextOptionsBuilder<T>> config)
    {
        DbContextOptionsBuilder<T> builder = config()
            .UseSqlServer(connectionString);

        return Activator.CreateInstance(typeof(T), builder.Options) as T
            ?? throw new Exception($"ContextBuilder<{typeof(T).Name}>.GetDbContext: Failed to initialize an instance of DbContext");
    }
}
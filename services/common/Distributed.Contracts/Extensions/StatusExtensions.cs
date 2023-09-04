using Distributed.Core.Schema;

namespace Distributed.Contracts.Extensions;
public static class StatusExtensions
{
    public static Status GenerateCreatedStatus<T>(this T entity) where T : Entity => new()
    {
        State = Statuses.Created,
        EntityId = entity.Id,
        EntityType = entity.Type,
        Context = $"Created {entity.Type}"
    };
}
namespace Distributed.Core.Entities;
public interface IDependency : IEntity
{
    int EntityId { get; set; }
    string EntityType { get; set; }
}
namespace Distributed.Core.Schema;
public interface IDependency : IEntity
{
    int EntityId { get; set; }
    string EntityType { get; set; }
}
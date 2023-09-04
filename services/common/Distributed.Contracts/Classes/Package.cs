using System.ComponentModel.DataAnnotations.Schema;
using Distributed.Core.Schema;

namespace Distributed.Contracts;

[Table("Package")]
public class Package : Entity, IDependency, IStateful<WorkflowStates>
{
    public int? WorkflowId { get; set; }
    public WorkflowStates State { get; set; }
    public Intents Intent { get; set; }
    public Statuses Result { get; set; } = Statuses.Active;
    public int EntityId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;    
}
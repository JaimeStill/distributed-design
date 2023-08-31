using System.ComponentModel.DataAnnotations.Schema;
using Distributed.Core.Schema;

namespace Distributed.Contracts;

[Table("Package")]
public class Package : Entity, IStateful<ProcessStates>
{
    public int? WorkflowId { get; set; }
    public ProcessStates State { get; set; }
    public Statuses Result { get; set; } = Statuses.Active;
    public string Context { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;    
}
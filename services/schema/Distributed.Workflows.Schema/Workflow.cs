using System.ComponentModel.DataAnnotations.Schema;
using Distributed.Contracts;
using Distributed.Core.Schema;

namespace Distributed.Workflows.Schema;

[Table("Workflow")]
public class Workflow : Entity, IStateful<WorkflowStates>
{
    public int PackageId { get; set; }
    public WorkflowStates State { get; set; }
    public string Description { get; set; } = string.Empty;
}
using System.ComponentModel.DataAnnotations.Schema;
using Distributed.Core.Entities;
using Workflows.Contracts;

namespace Workflows.Entities;

[Table("Workflow")]
public class Workflow : Entity, IStateful<WorkflowStates>
{
    public int PackageId { get; set; }
    public WorkflowStates State { get; set; }
    public string Description { get; set; } = string.Empty;
}
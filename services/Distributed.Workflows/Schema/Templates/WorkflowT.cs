using System.ComponentModel.DataAnnotations.Schema;
using Distributed.Core.Schema;

namespace Distributed.Workflows.Schema;

[Table("WorkflowTemplate")]
public class WorkflowTemplate : Entity, ITemplate<Workflow>
{
    public string Description { get; set; } = string.Empty;
}
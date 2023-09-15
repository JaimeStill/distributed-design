using System.ComponentModel.DataAnnotations.Schema;
using Distributed.Core.Entities;

namespace Workflows.Entities;

[Table("WorkflowTemplate")]
public class WorkflowTemplate : Entity, ITemplate<Workflow>
{
    public string Description { get; set; } = string.Empty;
}
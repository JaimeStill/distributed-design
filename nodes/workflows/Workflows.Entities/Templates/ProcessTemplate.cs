using System.ComponentModel.DataAnnotations.Schema;
using Distributed.Core.Entities;

namespace Workflows.Entities;

[Table("ProcessTemplate")]
public class ProcessTemplate : Entity, ITemplate<Process>
{
    public int WorkflowTemplateId { get; set; }
    public int Index { get; set; }
    public string Description { get; set; } = string.Empty;
}
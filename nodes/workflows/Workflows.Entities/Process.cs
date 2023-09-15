using System.ComponentModel.DataAnnotations.Schema;
using Distributed.Core.Entities;
using Workflows.Contracts;

namespace Workflows.Entities;

[Table("Process")]
public class Process : Entity
{
    public int WorkflowId { get; set; }
    public int Index { get; set; }
    public ProcessActions? Action { get; set; }
    public string Description { get; set; } = string.Empty;
}
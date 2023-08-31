using System.ComponentModel.DataAnnotations.Schema;
using Distributed.Contracts;
using Distributed.Core.Schema;

namespace Distributed.Workflows.Schema;

[Table("Process")]
public class Process : Entity, IStateful<ProcessStates>
{
    public int WorkflowId { get; set; }
    public ProcessStates State { get; set; }
    public string Description { get; set; } = string.Empty;
}
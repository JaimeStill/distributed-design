using System.ComponentModel.DataAnnotations.Schema;
using Distributed.Contracts;
using Distributed.Core.Schema;

namespace Distributed.Workflows.Schema;

[Table("Workflow")]
public class Workflow : Entity, IStateful<ProcessStates>
{
    public ProcessStates State { get; set; }
    public string Description { get; set; } = string.Empty;
}
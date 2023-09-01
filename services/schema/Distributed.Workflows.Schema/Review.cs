using System.ComponentModel.DataAnnotations.Schema;
using Distributed.Core.Schema;

namespace Distributed.Workflows.Schema;

[Table("Review")]
public class Review : Entity
{
    public int ProcessId { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool? Concur { get; set; }
}
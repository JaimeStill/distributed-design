using System.ComponentModel.DataAnnotations.Schema;
using Distributed.Core.Schema;

namespace Distributed.Workflows.Schema;

[Table("ReviewTemplate")]
public class ReviewTemplate : Entity, ITemplate<Review>
{
    public int ProcessTemplateId { get; set; }
    public string Description { get; set; } = string.Empty;
}
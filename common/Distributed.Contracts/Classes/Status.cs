using System.ComponentModel.DataAnnotations.Schema;
using Distributed.Core.Schema;

namespace Distributed.Contracts;

[Table("Status")]
public class Status : Entity, IDependency, IStateful<Statuses>
{
    public Statuses State { get; set; }
    public override string Value => State.ToString();
    public int EntityId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
}
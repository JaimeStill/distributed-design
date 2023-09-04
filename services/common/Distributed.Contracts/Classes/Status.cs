using System.ComponentModel.DataAnnotations.Schema;
using Distributed.Core.Schema;
using Microsoft.EntityFrameworkCore;

namespace Distributed.Contracts;

[Table("Status")]
public class Status : Entity, IDependency, IStateful<Statuses>
{
    public Statuses State { get; set; }
    public int EntityId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;

    public override void OnSaving(EntityState state)
    {
        base.OnSaving(state);
        Value = State.ToString();
    }
}
using System.ComponentModel.DataAnnotations.Schema;
using Distributed.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Distributed.Contracts;

[Table("Status")]
public class Status : Entity
{
    public Statuses State { get; set; }
    public int EntityId { get; set; }
    public string EntityType { get; set; } = string.Empty;

    public override void OnSaving(EntityState state)
    {
        base.OnSaving(state);
        Value = State.ToString();
    }
}
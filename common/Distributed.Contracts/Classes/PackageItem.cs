using System.ComponentModel.DataAnnotations.Schema;
using Distributed.Core.Schema;

namespace Distributed.Contracts;

[Table("PackageItem")]
public class PackageItem : Entity, IDependency, IStateful<Intents>
{
    public int PackageId { get; set; }
    public Intents State { get; set; }
    public int EntityId { get; set; }
    public string EntityType { get; set; } = string.Empty;
}
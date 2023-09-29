using System.ComponentModel.DataAnnotations.Schema;
using Distributed.Contracts;
using Distributed.Core.Entities;

namespace Workflows.Contracts;

[Table("Package")]
public class Package : Entity
{
    public PackageStates State { get; set; } = PackageStates.Pending;
    public Statuses Result { get; set; } = Statuses.Active;
    public int EntityId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;    
}
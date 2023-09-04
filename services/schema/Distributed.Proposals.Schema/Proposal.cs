using System.ComponentModel.DataAnnotations.Schema;
using Distributed.Core.Schema;

namespace Distributed.Proposals.Schema;

[Table("Proposal")]
public class Proposal : Entity
{
    public int StatusId { get; set; }
    public int? PackageId { get; set; }
    public string Title { get; set; } = string.Empty;
}
namespace Distributed.Core.Gateway;
public record Gateway
{
    public Guid Id { get; set; }
    public List<Endpoint> Endpoints { get; set; } = new();
}
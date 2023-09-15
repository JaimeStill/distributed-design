namespace Distributed.Core.Gateway;
public record GatewayOptions
{
    public const string Gateway = "Gateway";
    public Guid Id { get; set; }
    public List<Endpoint> Endpoints { get; set; } = new();
}
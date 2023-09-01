namespace Distributed.Core.Graph;
public record Endpoint
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
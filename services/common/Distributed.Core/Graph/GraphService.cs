using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Distributed.Core.Graph;
public class GraphService
{
    readonly Graph graph;

    public GraphService(IConfiguration config)
    {
        graph = config
            .GetSection("Graph")
            .Get<Graph>()
        ?? throw new Exception(
            $"Invalid Graph Configuration: configuration is missing or invalid. It must be provided when registering a Graph service."
        );
    }

    public GraphService(Graph graph)
    {
        this.graph = graph;
    }

    public Guid GraphId => graph.Id;
    public List<Endpoint> Endpoints => graph.Endpoints;

    public Endpoint GetEndpoint(string name) =>
        graph.Endpoints.First(x =>
            x.Name.ToLower() == name.ToLower()
        );
}

public static class GraphRegistration
{
    public static void AddGraphService(this IServiceCollection services) =>
        services.AddSingleton<GraphService>();
}
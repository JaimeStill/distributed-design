using Distributed.Core.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Distributed.Core.Graph;
[Route("graph")]
public abstract class GraphControllerBase : ApiController
{
    protected readonly GraphService graphSvc;

    public GraphControllerBase(GraphService svc)
    {
        graphSvc = svc;
    }

    [HttpGet]
    public IActionResult Get() => ApiResult(graphSvc.GraphId);
}
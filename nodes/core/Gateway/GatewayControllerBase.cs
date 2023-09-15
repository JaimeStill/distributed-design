using Distributed.Core.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Distributed.Core.Gateway;
[Route("gateway")]
public abstract class GatewayControllerBase : ApiController
{
    protected readonly GatewayService gatewaySvc;

    public GatewayControllerBase(GatewayService svc)
    {
        gatewaySvc = svc;
    }

    [HttpGet]
    public IActionResult Get() => ApiResult(gatewaySvc.GatewayId);
}
using Distributed.Core.Controllers;
using Microsoft.AspNetCore.Mvc;
using Workflows.Contracts;

namespace Proposals.Api.Controllers;

[Route("api/[controller]")]
public class WorkflowsController : ApiController
{
    readonly WorkflowsGateway gateway;

    public WorkflowsController(WorkflowsGateway gateway)
    {
        this.gateway = gateway;
    }

    [HttpGet("[action]/{type}")]
    public async Task<IActionResult> GetPackagesByType(
        [FromRoute] string type
    ) => ApiResult(await gateway.GetPackagesByType(type));

    [HttpGet("[action]/{id:int}/{type}")]
    public async Task<IActionResult> GetPackagesByEntity(
        [FromRoute] int id,
        [FromRoute] string type
    ) => ApiResult(await gateway.GetPackagesByEntity(id, type));

    [HttpGet("[action]/{id:int}/{type}")]
    public async Task<IActionResult> GetActivePackage(
        [FromRoute] int id,
        [FromRoute] string type
    ) => ApiResult(await gateway.GetActivePackage(id, type));

    [HttpPost("[action]")]
    public async Task<IActionResult> ValidatePackage(
        [FromBody]Package package
    ) => ApiResult(await gateway.ValidatePackage(package));

    [HttpPost("[action]")]
    public async Task<IActionResult> SubmitPackage(
        [FromBody]Package package
    ) => ApiResult(await gateway.SubmitPackage(package));

    [HttpPost("[action]")]
    public async Task<IActionResult> WithdrawPackage(
        [FromBody]Package package
    ) => ApiResult(await gateway.WithdrawPackage(package));
}
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

    [HttpGet("[action]/{entityType}")]
    public async Task<IActionResult> GetPackagesByType(
        [FromRoute] string entityType
    ) => ApiResult(await gateway.GetPackagesByType(entityType));

    [HttpGet("[action]/{id:int}/{entityType}")]
    public async Task<IActionResult> GetPackagesByEntity(
        [FromRoute] int id,
        [FromRoute] string entityType
    ) => ApiResult(await gateway.GetPackagesByEntity(id, entityType));

    [HttpGet("[action]/{id:int}/{entityType}")]
    public async Task<IActionResult> GetActivePackage(
        [FromRoute] int id,
        [FromRoute] string entityType
    ) => ApiResult(await gateway.GetActivePackage(id, entityType));

    [HttpPost("[action]")]
    public async Task<IActionResult> ValidatePackage(
        [FromBody]Package package
    ) => ApiResult(await gateway.ValidatePackage(package));

    [HttpPost("[action]")]
    public async Task<IActionResult> SubmitPackage(
        [FromBody] Package package
    ) => ApiResult(await gateway.SubmitPackage(package));

    [HttpPost("[action]")]
    public async Task<IActionResult> ResubmitPackage(
        [FromBody] Package package
    ) => ApiResult(await gateway.ResubmitPackage(package));

    [HttpPost("[action]")]
    public async Task<IActionResult> WithdrawPackage(
        [FromBody] Package package
    ) => ApiResult(await gateway.WithdrawPackage(package));
}
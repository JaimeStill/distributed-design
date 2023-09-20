using Distributed.Core.Gateway;
using Microsoft.AspNetCore.Mvc;
using Workflows.Contracts;
using Workflows.Services;

namespace Workflows.Api.Controllers;
public class GatewayController : GatewayControllerBase
{
    readonly PackageQuery packageQuery;
    readonly PackageCommand packageCommand;

    public GatewayController(
        GatewayService gateway,
        PackageQuery packageQuery,
        PackageCommand packageCommand
    )
    : base(gateway)
    {
        this.packageQuery = packageQuery;
        this.packageCommand = packageCommand;
    }

    [HttpGet("[action]/{entityType}")]
    public async Task<IActionResult> GetPackagesByType(
        [FromRoute] string entityType
    ) => ApiResult(await packageQuery.GetByType(entityType));

    [HttpGet("[action]/{id:int}/{entityType}")]
    public async Task<IActionResult> GetPackagesByEntity(
        [FromRoute] int id,
        [FromRoute] string entityType
    ) => ApiResult(await packageQuery.GetByEntity(id, entityType));

    [HttpGet("[action]/{id:int}/{entityType}")]
    public async Task<IActionResult> GetActivePackage(
        [FromRoute] int id,
        [FromRoute] string entityType
    ) => ApiResult(await packageQuery.GetActivePackage(id, entityType));

    [HttpPost("[action]")]
    public async Task<IActionResult> ValidatePackage([FromBody] Package package) =>
        ApiResult(await packageCommand.Validate(package));

    [HttpPost("[action]")]
    public async Task<IActionResult> SubmitPackage([FromBody] Package package) =>
        ApiResult(await packageCommand.Save(package));

    [HttpPost("[action]")]
    public async Task<IActionResult> WithdrawPackage([FromBody] Package package) =>
        ApiResult(await packageCommand.Remove(package));
}
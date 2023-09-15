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

    [HttpGet("[action]/{id:int}/{type}")]
    public async Task<IActionResult> GetPackage(
        [FromRoute] int id,
        [FromRoute] string type
    ) => ApiResult(await packageQuery.GetByEntity(id, type));

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
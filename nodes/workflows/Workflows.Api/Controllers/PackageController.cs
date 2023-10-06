using Distributed.Core.Controllers;
using Microsoft.AspNetCore.Mvc;
using Workflows.Contracts;
using Workflows.Services;

namespace Workflows.Api.Controllers;

[Route("api/[controller]")]
public class PackageController : EntityController<Package, PackageQuery, PackageCommand>
{
    readonly PackageQuery query;
    readonly PackageCommand command;

    public PackageController(PackageQuery query, PackageCommand command)
    : base(query, command)
    {
        this.query = query;
        this.command = command;
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetActive() =>
        ApiResult(await query.GetActive());

    [HttpGet("[action]")]
    public async Task<IActionResult> GetCompleted() =>
        ApiResult(await query.GetCompleted());

    [HttpGet("[action]/{entityType}")]
    public async Task<IActionResult> GetByType(
        [FromRoute] string entityType
    ) => ApiResult(await query.GetByType(entityType));

    [HttpGet("[action]/{id:int}/{entityType}")]
    public async Task<IActionResult> GetByEntity(
        [FromRoute] int id,
        [FromRoute] string entityType
    ) => ApiResult(await query.GetByEntity(id, entityType));

    [HttpGet("[action]/{id:int}/{entityType}")]
    public async Task<IActionResult> GetActivePackage(
        [FromRoute] int id,
        [FromRoute] string entityType
    ) => ApiResult(await query.GetActivePackage(id, entityType));

    [HttpPost("[action]")]
    public async Task<IActionResult> Approve(
        [FromBody] Package package
    ) => ApiResult(await command.Approve(package));

    [HttpPost("[action]")]
    public async Task<IActionResult> Reject(
        [FromBody] Package package
    ) => ApiResult(await command.Reject(package));

    [HttpPost("[action]")]
    public async Task<IActionResult> Return(
        [FromBody] Package package
    ) => ApiResult(await command.Return(package));
}
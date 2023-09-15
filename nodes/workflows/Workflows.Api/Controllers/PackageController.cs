using Distributed.Core.Controllers;
using Microsoft.AspNetCore.Mvc;
using Workflows.Contracts;
using Workflows.Services;

namespace Workflows.Api.Controllers;

[Route("api/[controller]")]
public class PackageController : EntityController<Package, PackageQuery, PackageCommand>
{
    readonly PackageQuery query;

    public PackageController(PackageQuery query, PackageCommand command)
    : base(query, command)
    {
        this.query = query;
    }

    [HttpGet("[action]/{id:int}/{type}")]
    public async Task<IActionResult> GetByEntity(
        [FromRoute] int id,
        [FromRoute] string type
    ) => ApiResult(await query.GetByEntity(id, type));

    [HttpGet("[action]/{id:int}")]
    public async Task<IActionResult> GetByWorkflow([FromRoute]int id) =>
        ApiResult(await query.GetByWorkflow(id));
}
using Distributed.Core.Controllers;
using Microsoft.AspNetCore.Mvc;
using Proposals.Services;
using Proposals.Entities;

namespace Proposals.Api.Controllers;

[Route("api/[controller]")]
public class ProposalController : EntityController<Proposal, ProposalQuery, ProposalCommand>
{
    readonly ProposalQuery query;

    public ProposalController(ProposalQuery query, ProposalCommand command)
    : base(query, command)
    {
        this.query = query;
    }

    [HttpGet("[action]/{id:int}")]
    public async Task<IActionResult> GetStatus([FromRoute]int id) =>
        ApiResult(await query.GetStatus(id));
}
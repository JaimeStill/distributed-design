using Distributed.Core.Controllers;
using Distributed.Proposals.Logic;
using Distributed.Proposals.Schema;
using Microsoft.AspNetCore.Mvc;

namespace Distributed.Proposals.Api.Controllers;

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
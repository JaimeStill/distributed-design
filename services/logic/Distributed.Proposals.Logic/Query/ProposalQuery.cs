using Distributed.Contracts;
using Distributed.Core.Services;
using Distributed.Proposals.Data;
using Distributed.Proposals.Schema;
using Microsoft.EntityFrameworkCore;

namespace Distributed.Proposals.Logic;
public class ProposalQuery : EntityQuery<Proposal, ProposalsContext>
{
    public ProposalQuery(ProposalsContext db) : base(db)
    { }

    public async Task<Status?> GetStatus(int id)
    {
        int? statusId = await db.Proposals
            .Where(x => x.Id == id)
            .Select(x => x.StatusId)
            .FirstOrDefaultAsync();

        return statusId is null
            ? default
            : await db.Statuses.FindAsync(statusId);
    }
}
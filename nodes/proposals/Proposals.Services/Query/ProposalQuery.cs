using Distributed.Contracts;
using Distributed.Core.Services;
using Microsoft.EntityFrameworkCore;
using Proposals.Data;
using Proposals.Entities;

namespace Proposals.Services;
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
using Distributed.Core.Services;
using Microsoft.EntityFrameworkCore;
using Proposals.Data;
using Proposals.Entities;
using Workflows.Contracts;

namespace Proposals.Services;
public class PackageSaga : EntitySaga<Package, ProposalsContext>
{
    public PackageSaga(ProposalsContext db) : base(db)
    { }

    public async Task OnAdd(Package package)
    {
        Proposal? proposal =
            await db.Proposals
                .FirstOrDefaultAsync(x =>
                    x.Type == package.EntityType
                    && x.Id == package.EntityId
                );

        if (proposal is not null)
        {
            db.Proposals.Attach(proposal);
            proposal.PackageId = package.Id;
            await db.SaveChangesAsync();
        }
    }

    public async Task OnRemove(Package package)
    {
        Proposal? proposal =
            await db.Proposals
                .FirstOrDefaultAsync(x =>
                    x.Type == package.EntityType
                    && x.Id == package.EntityId
                    && x.PackageId == package.Id
                );

        if (proposal is not null)
        {
            db.Proposals.Attach(proposal);
            proposal.PackageId = null;
            await db.SaveChangesAsync();
        }
    }
}
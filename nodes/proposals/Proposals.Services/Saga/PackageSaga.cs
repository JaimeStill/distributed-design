using Distributed.Contracts;
using Distributed.Core.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Proposals.Data;
using Proposals.Entities;
using Workflows.Contracts;

namespace Proposals.Services;
public class PackageSaga : EntitySaga<Package, ProposalsContext>
{
    readonly IHubContext<ProposalEventHub, IProposalEventHub> proposalEvents;
    public PackageSaga(
        ProposalsContext db,
        IHubContext<ProposalEventHub, IProposalEventHub> proposalEvents
    ) : base(db)
    {
        this.proposalEvents = proposalEvents;
    }

    public async Task OnAdd(Package package)
    {
        Proposal? proposal = await FindByPackage(package);

        if (proposal is not null)
        {
            db.Proposals.Attach(proposal);
            proposal.PackageId = package.Id;
            await db.SaveChangesAsync();
            
            await SendProposalEvent(proposal, "updated");
        }
    }

    public async Task OnRemove(Package package)
    {
        Proposal? proposal = await FindByPackage(package);

        if (proposal?.PackageId == package.Id)
        {
            db.Proposals.Attach(proposal);
            proposal.PackageId = null;
            await db.SaveChangesAsync();

            await SendProposalEvent(proposal, "updated");
        }
    }

    public async Task OnStateChanged(Package package)
    {
        if (package.State == PackageStates.Approved || package.State == PackageStates.Rejected)
        {
            Proposal? proposal = await FindByPackage(package);

            if (proposal?.PackageId == package.Id)
            {
                db.Proposals.Attach(proposal);
                proposal.PackageId = null;

                if (package.State == PackageStates.Approved)
                    await UpdateStatus(proposal, package);

                await db.SaveChangesAsync();

                await SendProposalEvent(proposal, "updated");
            }
        }
    }

    async Task SendProposalEvent(Proposal proposal, string action)
    {

        EventMessage<Proposal> message = GenerateMessage(proposal, action);

        await proposalEvents
            .Clients
            .All
            .OnUpdate(message);
    }

    static EventMessage<Proposal> GenerateMessage(Proposal proposal, string action)
    {
        EventMessage<Proposal> message = new(
            proposal,
            $"Proposal {proposal.Title} successfully {action}"
        );

        return message;
    }

    async Task UpdateStatus(Proposal proposal, Package package)
    {
        Status? status = await db.Statuses.FindAsync(proposal.StatusId);

        if (status is not null)
        {
            db.Statuses.Attach(status);

            status.State = package.State == PackageStates.Approved
                ? package.Result
                : Statuses.Rejected;
        }
    }

    async Task<Proposal?> FindByPackage(Package package) =>
        await db.Proposals
            .FirstOrDefaultAsync(x =>
                x.Type == package.EntityType
                && x.Id == package.EntityId
            );
}
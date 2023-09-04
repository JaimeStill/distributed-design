using Distributed.Contracts;
using Distributed.Contracts.Extensions;
using Distributed.Core.Messages;
using Distributed.Core.Services;
using Distributed.Proposals.Data;
using Distributed.Proposals.Schema;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Distributed.Proposals.Logic;
public class ProposalCommand : EntityCommand<Proposal, ProposalEventHub, IProposalEventHub, ProposalsContext>
{
    public ProposalCommand(ProposalsContext db, IHubContext<ProposalEventHub, IProposalEventHub> events)
    : base(db, events)
    { }

    protected override Func<Proposal, Task<Proposal>>? AfterAdd => async (Proposal proposal) =>
    {
        Status status = proposal.GenerateCreatedStatus();
        await db.Statuses.AddAsync(status);
        await db.SaveChangesAsync();

        Set.Attach(proposal);
        proposal.StatusId = status.Id;
        await db.SaveChangesAsync();

        return proposal;
    };

    async Task<bool> ValidateStatus(Proposal proposal) =>
        !await Set
            .AnyAsync(x =>
                x.Id == proposal.Id
                && x.StatusId != proposal.StatusId
            );

    public override async Task<ValidationMessage> Validate(Proposal proposal)
    {
        ValidationMessage result = await base.Validate(proposal);

        if (!await ValidateStatus(proposal))
            result.AddMessage("Status cannot be modified");

        return result;
    }
}
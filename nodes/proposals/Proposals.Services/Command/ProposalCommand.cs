using Distributed.Contracts;
using Distributed.Contracts.Extensions;
using Distributed.Core.Messages;
using Distributed.Core.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Proposals.Data;
using Proposals.Entities;
using Workflows.Contracts;

namespace Proposals.Services;
public class ProposalCommand : EntityCommand<Proposal, ProposalEventHub, IProposalEventHub, ProposalsContext>
{
    readonly WorkflowsGateway workflows;
    public ProposalCommand(
        WorkflowsGateway workflows,
        ProposalsContext db,
        IHubContext<ProposalEventHub,
        IProposalEventHub> events
    )
    : base(db, events)
    {
        this.workflows = workflows;
    }

    protected override Func<Proposal, Task<HookMessage<Proposal>>>? AfterAdd => async (Proposal proposal) =>
    {
        try
        {
            Status status = proposal.GenerateCreatedStatus();
            await db.Statuses.AddAsync(status);
            await db.SaveChangesAsync();

            Set.Attach(proposal);
            proposal.StatusId = status.Id;
            await db.SaveChangesAsync();

            return new(proposal);
        }
        catch (Exception ex)
        {
            return new(proposal, ex);
        }
    };

    protected override Func<Proposal, Task<HookMessage<Proposal>>>? AfterRemove => async (Proposal proposal) =>
    {
        try
        {
            Package? package = await workflows.GetActivePackage(proposal.Id, proposal.Type);
            
            if (package is not null)
            {
                ApiMessage<int>? message = await workflows.WithdrawPackage(package);

                if (message is null)
                    return new(proposal, new Exception("Unable to withdraw associated Package"));
                else if (message.Error)
                    return new(proposal, new Exception(message.Message));
            }

            return new(proposal);
        }
        catch (Exception ex)
        {
            return new(proposal, ex);
        }
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
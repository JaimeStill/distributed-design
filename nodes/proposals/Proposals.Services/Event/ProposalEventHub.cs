using Distributed.Core.Services;
using Proposals.Entities;

namespace Proposals.Services;
public class ProposalEventHub : EventHub<Proposal, IProposalEventHub>
{ }
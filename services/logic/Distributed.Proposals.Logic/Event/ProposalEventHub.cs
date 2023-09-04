using Distributed.Core.Services;
using Distributed.Proposals.Schema;

namespace Distributed.Proposals.Logic;
public class ProposalEventHub : EventHub<Proposal, IProposalEventHub>
{ }
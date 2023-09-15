using Distributed.Core.Services;
using Proposals.Entities;

namespace Proposals.Services;
public interface IProposalEventHub : IEventHub<Proposal>
{ }
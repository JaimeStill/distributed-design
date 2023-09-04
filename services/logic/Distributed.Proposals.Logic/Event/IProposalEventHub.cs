using Distributed.Core.Services;
using Distributed.Proposals.Schema;

namespace Distributed.Proposals.Logic;
public interface IProposalEventHub : IEventHub<Proposal>
{ }
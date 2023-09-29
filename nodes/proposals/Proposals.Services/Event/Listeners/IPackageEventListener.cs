using Distributed.Core.Services;
using Workflows.Contracts;

namespace Proposals.Services;
public interface IPackageEventListener : IEventListener<Package>
{
    EventAction OnStateChanged { get; }
}
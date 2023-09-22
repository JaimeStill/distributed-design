using Distributed.Core.Services;
using Proposals.Services;
using Workflows.Contracts;

namespace Proposals.Api.Events;
public class PackageEventStartup : EventStartup<PackageEventListener, Package>
{
    public PackageEventStartup(
        IServiceProvider provider
    ) : base(provider) { }
}
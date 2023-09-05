using Distributed.Contracts;
using Distributed.Core.Services;

namespace Distributed.Workflows.Logic;
public interface IPackageEventHub : IEventHub<Package>
{
    Task OnComplete(IEventMessage<Package> message);
}
using Distributed.Core.Services;
using Microsoft.AspNetCore.SignalR;
using Workflows.Data;
using Workflows.Contracts;

namespace Workflows.Services;
public class PackageCommand : EntityCommand<Package, PackageEventHub, IPackageEventHub, WorkflowsContext>
{
    public PackageCommand(WorkflowsContext db, IHubContext<PackageEventHub, IPackageEventHub> events)
    : base(db, events)
    { }
}
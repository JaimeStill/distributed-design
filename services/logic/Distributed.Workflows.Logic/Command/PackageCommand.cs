using Distributed.Contracts;
using Distributed.Core.Services;
using Distributed.Workflows.Data;
using Microsoft.AspNetCore.SignalR;

namespace Distributed.Workflows.Logic;
public class PackageCommand : EntityCommand<Package, PackageEventHub, IPackageEventHub, WorkflowsContext>
{
    public PackageCommand(WorkflowsContext db, IHubContext<PackageEventHub, IPackageEventHub> events)
    : base(db, events)
    { }
}
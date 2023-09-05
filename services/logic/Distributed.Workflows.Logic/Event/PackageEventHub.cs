using Distributed.Contracts;
using Distributed.Core.Services;

namespace Distributed.Workflows.Logic;
public class PackageEventHub : EventHub<Package, IPackageEventHub>
{ }
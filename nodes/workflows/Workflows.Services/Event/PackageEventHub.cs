using Distributed.Core.Services;
using Workflows.Contracts;

namespace Workflows.Services;
public class PackageEventHub : EventHub<Package, IPackageEventHub>
{ }
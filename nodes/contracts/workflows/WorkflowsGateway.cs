using Distributed.Core.Gateway;
using Distributed.Core.Messages;

namespace Workflows.Contracts;
public class WorkflowsGateway : GatewayClient
{
    public WorkflowsGateway(GatewayService gateway)
    : base(gateway, "Workflows")
    { }

    public async Task<List<Package>> GetPackagesByType(string entityType) =>
        await Get<List<Package>>($"getPackagesByType/{entityType}")
        ?? new();

    public async Task<List<Package>> GetPackagesByEntity(int id, string entityType) =>
        await Get<List<Package>>($"getPackagesByEntity/{id}/{entityType}")
        ?? new();

    public async Task<Package?> GetActivePackage(int id, string entityType) =>
        await Get<Package?>($"getActivePackage/{id}/{entityType}");

    public async Task<ValidationMessage?> ValidatePackage(Package package) =>
        await Post<ValidationMessage, Package>(package, "validatePackage");

    public async Task<ApiMessage<Package>?> SubmitPackage(Package package) =>
        await Post<ApiMessage<Package>, Package>(package, "submitPackage");

    public async Task<ApiMessage<int>?> WithdrawPackage(Package package) =>
        await Post<ApiMessage<int>, Package>(package, "withdrawPackage");
}
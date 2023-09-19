using Distributed.Core.Gateway;
using Distributed.Core.Messages;

namespace Workflows.Contracts;
public class WorkflowsGateway : GatewayClient
{
    public WorkflowsGateway(GatewayService gateway)
    : base(gateway, "Workflows")
    { }

    public async Task<List<Package>> GetPackagesByType(string type) =>
        await Get<List<Package>>($"getPackagesByType/{type}")
        ?? new();

    public async Task<List<Package>> GetPackagesByEntity(int id, string type) =>
        await Get<List<Package>>($"getPackagesByEntity/{id}/{type}")
        ?? new();

    public async Task<Package?> GetActivePackage(int id, string type) =>
        await Get<Package?>($"getActivePackage/{id}/{type}");

    public async Task<ValidationMessage?> ValidatePackage(Package package) =>
        await Post<ValidationMessage, Package>(package, "validatePackage");

    public async Task<ApiMessage<Package>?> SubmitPackage(Package package) =>
        await Post<ApiMessage<Package>, Package>(package, "submitPackage");

    public async Task<ApiMessage<int>?> WithdrawPackage(Package package) =>
        await Delete<ApiMessage<int>, Package>(package, "withdrawPackage");
}
using Distributed.Core.Gateway;
using Distributed.Core.Messages;

namespace Distributed.Contracts.Gateways;
public class WorkflowsGateway : GatewayClient
{
    public WorkflowsGateway(GatewayService gateway)
    : base(gateway, "Workflows")
    { }

    public async Task<Package?> GetPackage(int id, string type) =>
        await Get<Package?>($"getPackage/{id}/{type}");

    public async Task<ValidationMessage?> ValidatePackage(Package package) =>
        await Post<ValidationMessage, Package>(package, "validatePackage");

    public async Task<ApiMessage<Package>?> SubmitPackage(Package package) =>
        await Post<ApiMessage<Package>, Package>(package, "submitPackage");

    public async Task<ApiMessage<int>?> WithdrawPackage(Package package) =>
        await Delete<ApiMessage<int>, Package>(package, "withdrawPackage");
}
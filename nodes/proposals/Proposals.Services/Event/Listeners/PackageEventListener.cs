using Distributed.Core.Extensions;
using Distributed.Core.Services;
using Microsoft.Extensions.Configuration;
using Workflows.Contracts;

namespace Proposals.Services;
public class PackageEventListener : EventListener<Package, PackageSaga>, IPackageEventListener
{
    public EventAction OnComplete { get; }
    public PackageEventListener(IServiceProvider provider, IConfiguration config)
    : base(
        provider,
        config.GetEventEndpoint("Package")
    )
    {
        OnComplete = new("OnComplete", connection);

        OnAdd.Set(
            HandleEvent(async (EventMessage<Package> message, PackageSaga saga) =>
                await saga.OnAdd(message.Data)
            )
        );

        OnRemove.Set(
            HandleEvent(async (EventMessage<Package> message, PackageSaga saga) =>
                await saga.OnRemove(message.Data)
            )
        );
    }

    protected override void DisposeEvents()
    {
        base.DisposeEvents();
        OnComplete.Dispose();
    }
}
using Distributed.Core.Services;
using Microsoft.AspNetCore.SignalR;
using Workflows.Data;
using Workflows.Contracts;
using Microsoft.EntityFrameworkCore;
using Distributed.Core.Messages;

namespace Workflows.Services;
public class PackageCommand : EntityCommand<Package, PackageEventHub, IPackageEventHub, WorkflowsContext>
{
    public PackageCommand(WorkflowsContext db, IHubContext<PackageEventHub, IPackageEventHub> events)
    : base(db, events)
    { }

    #region Public

    public async Task<ApiMessage<Package>> Submit(Package package) =>
        package.Id > 0
            ? await ChangeState(package, PackageStates.Pending)
            : await Save(package);

    public async Task<ApiMessage<Package>> Approve(Package package) =>
        await ChangeState(package, PackageStates.Approved);

    public async Task<ApiMessage<Package>> Reject(Package package) =>
        await ChangeState(package, PackageStates.Rejected);

    public async Task<ApiMessage<Package>> Return(Package package) =>
        await ChangeState(package, PackageStates.Returned);

    public override async Task<ValidationMessage> Validate(Package package)
    {
        ValidationMessage result = await base.Validate(package);

        if (!await ValidateResult(package))
            result.AddMessage("Result cannot be modified");

        if (!await ValidateState(package))
            result.AddMessage("State cannot be modified");

        if (!await ValidateEntity(package))
            result.AddMessage($"A Package already exists for {package.EntityType}.{package.EntityId}");

        return result;
    }

    #endregion

    #region Internal

    #region State Change
    
    Func<Package, Task> SyncState => async (Package package) =>
    {
        EventMessage<Package> message = GenerateMessage(package, $"changed state to {package.State}");

        await events
            .Clients
            .All
            .OnStateChanged(message);
    };

    async Task<ApiMessage<Package>> ChangeState(Package package, PackageStates state)
    {
        try
        {
            ValidationMessage validity = ValidateStateChange(package, state);

            if (validity.IsValid)
            {
                Set.Attach(package);
                package.State = state;
                await db.SaveChangesAsync();

                await SyncState(package);

                return new(package, $"{typeof(Package).Name} successfully changed state to {state}");
            }
            else
                return new(validity);
        }
        catch (Exception ex)
        {
            return new ("ChangeStatus", ex);
        }
    }

    static ValidationMessage FromPending(Package package, PackageStates state) => state switch
    {
        PackageStates.Pending => new($"Cannot transition from {package.State} to {state}"),
        _ => new()
    };

    static ValidationMessage FromReturned(Package package, PackageStates state) => state switch
    {
        PackageStates.Pending => new(),
        _ => new($"Cannot transition from {package.State} to {state}")
    };

    static ValidationMessage InvalidState(Package package) =>
        new($"Package {package.Title} with state {package.State} cannot be modified");

    static ValidationMessage ValidateStateChange(Package package, PackageStates state) => package.State switch
    {
        PackageStates.Pending => FromPending(package, state),
        PackageStates.Returned => FromReturned(package, state),
        _ => InvalidState(package)
    };

    #endregion

    #region Save Validation

    async Task<bool> ValidateResult(Package package) =>
        !await Set
            .AnyAsync(x =>
                x.Id == package.Id
                && x.Result != package.Result
            );

    async Task<bool> ValidateState(Package package) =>
        !await Set
            .AnyAsync(x =>
                x.Id == package.Id
                && x.State != package.State
            );

    async Task<bool> ValidateEntity(Package package) =>
        !await Set
            .AnyAsync(x =>
                x.EntityId == package.EntityId
                && x.EntityType == package.EntityType
                && (
                    x.State == PackageStates.Pending
                    || x.State == PackageStates.Returned
                )
            );

    #endregion

    #endregion
}
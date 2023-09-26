using Distributed.Core.Services;
using Microsoft.EntityFrameworkCore;
using Workflows.Contracts;
using Workflows.Data;

namespace Workflows.Services;
public class PackageQuery : EntityQuery<Package, WorkflowsContext>
{
    public PackageQuery(WorkflowsContext db) : base(db)
    { }

    public async Task<List<Package>> GetByType(string entityType) =>
        await Set
            .Where(x =>
                x.EntityType == entityType
            )
            .ToListAsync();

    public async Task<List<Package>> GetByEntity(int id, string entityType) =>
        await Set
            .Where(x =>
                x.EntityId == id
                && x.EntityType == entityType
            )
            .ToListAsync();

    public async Task<Package?> GetActivePackage(int id, string entityType) =>
        await Set
            .FirstOrDefaultAsync(x =>
                x.EntityId == id
                && x.EntityType == entityType
                && (
                    x.State == PackageStates.Pending
                    || x.State == PackageStates.Returned
                )
            );
}
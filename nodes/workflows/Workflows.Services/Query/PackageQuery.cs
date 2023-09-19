using Distributed.Core.Services;
using Microsoft.EntityFrameworkCore;
using Workflows.Contracts;
using Workflows.Data;

namespace Workflows.Services;
public class PackageQuery : EntityQuery<Package, WorkflowsContext>
{
    public PackageQuery(WorkflowsContext db) : base(db)
    { }

    public async Task<List<Package>> GetByType(string type) =>
        await Set
            .Where(x =>
                x.Type == type
            )
            .ToListAsync();

    public async Task<List<Package>> GetByEntity(int id, string type) =>
        await Set
            .Where(x =>
                x.EntityId == id
                && x.Type == type
            )
            .ToListAsync();

    public async Task<Package?> GetActivePackage(int id, string type) =>
        await Set
            .FirstOrDefaultAsync(x =>
                x.EntityId == id
                && x.Type == type
                && (
                    x.State == WorkflowStates.Pending
                    || x.State == WorkflowStates.Returned
                )
            );

    public async Task<Package?> GetByWorkflow(int id) =>
        await Set
            .FirstOrDefaultAsync(x =>
                x.WorkflowId == id
            );
}
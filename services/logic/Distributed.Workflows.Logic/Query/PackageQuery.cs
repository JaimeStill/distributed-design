using Distributed.Contracts;
using Distributed.Core.Services;
using Distributed.Workflows.Data;
using Microsoft.EntityFrameworkCore;

namespace Distributed.Workflows.Logic;
public class PackageQuery : EntityQuery<Package, WorkflowsContext>
{
    public PackageQuery(WorkflowsContext db) : base(db)
    { }

    public async Task<Package?> GetByEntity(int id, string type) =>
        await Set
            .FirstOrDefaultAsync(x =>
                x.EntityId == id
                && x.Type == type
            );

    public async Task<Package?> GetByWorkflow(int id) =>
        await Set
            .FirstOrDefaultAsync(x =>
                x.WorkflowId == id
            );
}
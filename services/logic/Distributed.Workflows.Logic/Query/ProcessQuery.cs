using Distributed.Core.Services;
using Distributed.Workflows.Data;
using Distributed.Workflows.Schema;
using Microsoft.EntityFrameworkCore;

namespace Distributed.Workflows.Logic;
public class ProcessQuery : EntityQuery<Process, WorkflowsContext>
{
    public ProcessQuery(WorkflowsContext db) : base(db)
    { }

    public override async Task<List<Process>> Get() =>
        await Set
            .OrderBy(x => x.WorkflowId)
                .ThenBy(x => x.Index)
            .ToListAsync();

    public async Task<List<Process>> GetByWorkflow(int id) =>
        await Set
            .Where(x => x.WorkflowId == id)
            .OrderBy(x => x.Index)
            .ToListAsync();
}
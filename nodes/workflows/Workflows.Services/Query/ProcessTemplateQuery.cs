using Distributed.Core.Services;
using Microsoft.EntityFrameworkCore;
using Workflows.Data;
using Workflows.Entities;

namespace Workflows.Services;
public class ProcessTemplateQuery : EntityQuery<ProcessTemplate, WorkflowsContext>
{
    public ProcessTemplateQuery(WorkflowsContext db) : base(db)
    { }

    public override async Task<List<ProcessTemplate>> Get() =>
        await Set
            .OrderBy(x => x.WorkflowTemplateId)
                .ThenBy(x => x.Index)
            .ToListAsync();

    public async Task<List<ProcessTemplate>> GetByWorkflow(int id) =>
        await Set
            .Where(x => x.WorkflowTemplateId == id)
            .OrderBy(x => x.Index)
            .ToListAsync();
}
using Distributed.Core.Services;
using Workflows.Data;
using Workflows.Entities;

namespace Workflows.Services;
public class WorkflowQuery : EntityQuery<Workflow, WorkflowsContext>
{
    public WorkflowQuery(WorkflowsContext db) : base(db)
    { }
}
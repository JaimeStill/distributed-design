using Distributed.Core.Services;
using Distributed.Workflows.Data;
using Distributed.Workflows.Schema;
using Microsoft.EntityFrameworkCore;

namespace Distributed.Workflows.Logic;
public class WorkflowQuery : EntityQuery<Workflow, WorkflowsContext>
{
    public WorkflowQuery(WorkflowsContext db) : base(db)
    { }
}
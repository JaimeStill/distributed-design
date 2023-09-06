using Distributed.Core.Services;
using Distributed.Workflows.Data;
using Distributed.Workflows.Schema;

namespace Distributed.Workflows.Logic;
public class WorkflowTemplateQuery : EntityQuery<WorkflowTemplate, WorkflowsContext>
{
    public WorkflowTemplateQuery(WorkflowsContext db) : base(db)
    { }
}
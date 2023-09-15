using Distributed.Core.Services;
using Workflows.Data;
using Workflows.Entities;

namespace Workflows.Services;
public class WorkflowTemplateQuery : EntityQuery<WorkflowTemplate, WorkflowsContext>
{
    public WorkflowTemplateQuery(WorkflowsContext db) : base(db)
    { }
}
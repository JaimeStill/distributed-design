using Distributed.Core.Services;
using Microsoft.AspNetCore.SignalR;
using Workflows.Data;
using Workflows.Entities;

namespace Workflows.Services;
public class WorkflowCommand : EntityCommand<Workflow, WorkflowEventHub, IWorkflowEventHub, WorkflowsContext>
{
    public WorkflowCommand(WorkflowsContext db, IHubContext<WorkflowEventHub, IWorkflowEventHub> events)
    : base(db, events)
    { }
}
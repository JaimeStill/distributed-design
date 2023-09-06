using Distributed.Core.Services;
using Distributed.Workflows.Data;
using Distributed.Workflows.Schema;
using Microsoft.AspNetCore.SignalR;

namespace Distributed.Workflows.Logic;
public class WorkflowCommand : EntityCommand<Workflow, WorkflowEventHub, IWorkflowEventHub, WorkflowsContext>
{
    public WorkflowCommand(WorkflowsContext db, IHubContext<WorkflowEventHub, IWorkflowEventHub> events)
    : base(db, events)
    { }
}
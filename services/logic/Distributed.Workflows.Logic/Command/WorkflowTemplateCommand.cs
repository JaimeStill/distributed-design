using Distributed.Core.Services;
using Distributed.Workflows.Data;
using Distributed.Workflows.Schema;
using Microsoft.AspNetCore.SignalR;

namespace Distributed.Workflows.Logic;
public class WorkflowTemplateCommand : EntityCommand<WorkflowTemplate, WorkflowTemplateEventHub, IWorkflowTemplateEventHub, WorkflowsContext>
{
    public WorkflowTemplateCommand(WorkflowsContext db, IHubContext<WorkflowTemplateEventHub, IWorkflowTemplateEventHub> events)
    : base(db, events)
    { }
}
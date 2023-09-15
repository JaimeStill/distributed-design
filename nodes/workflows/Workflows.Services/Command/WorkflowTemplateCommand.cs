using Distributed.Core.Services;
using Microsoft.AspNetCore.SignalR;
using Workflows.Data;
using Workflows.Entities;

namespace Workflows.Services;
public class WorkflowTemplateCommand : EntityCommand<WorkflowTemplate, WorkflowTemplateEventHub, IWorkflowTemplateEventHub, WorkflowsContext>
{
    public WorkflowTemplateCommand(WorkflowsContext db, IHubContext<WorkflowTemplateEventHub, IWorkflowTemplateEventHub> events)
    : base(db, events)
    { }
}
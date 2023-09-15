using Distributed.Core.Services;
using Microsoft.AspNetCore.SignalR;
using Workflows.Data;
using Workflows.Entities;

namespace Workflows.Services;
public class ProcessTemplateCommand : EntityCommand<ProcessTemplate, ProcessTemplateEventHub, IProcessTemplateEventHub, WorkflowsContext>
{
    public ProcessTemplateCommand(WorkflowsContext db, IHubContext<ProcessTemplateEventHub, IProcessTemplateEventHub> events)
    : base(db, events)
    { }
}
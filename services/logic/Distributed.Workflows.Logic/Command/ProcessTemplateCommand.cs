using Distributed.Core.Services;
using Distributed.Workflows.Data;
using Distributed.Workflows.Schema;
using Microsoft.AspNetCore.SignalR;

namespace Distributed.Workflows.Logic;
public class ProcessTemplateCommand : EntityCommand<ProcessTemplate, ProcessTemplateEventHub, IProcessTemplateEventHub, WorkflowsContext>
{
    public ProcessTemplateCommand(WorkflowsContext db, IHubContext<ProcessTemplateEventHub, IProcessTemplateEventHub> events)
    : base(db, events)
    { }
}
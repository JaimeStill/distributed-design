using Distributed.Core.Services;
using Microsoft.AspNetCore.SignalR;
using Workflows.Data;
using Workflows.Entities;

namespace Workflows.Services;
public class ProcessCommand : EntityCommand<Process, ProcessEventHub, IProcessEventHub, WorkflowsContext>
{
    public ProcessCommand(WorkflowsContext db, IHubContext<ProcessEventHub, IProcessEventHub> events)
    : base(db, events)
    { }
}
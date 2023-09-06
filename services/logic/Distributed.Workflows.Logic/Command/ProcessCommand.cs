using Distributed.Core.Services;
using Distributed.Workflows.Data;
using Distributed.Workflows.Schema;
using Microsoft.AspNetCore.SignalR;

namespace Distributed.Workflows.Logic;
public class ProcessCommand : EntityCommand<Process, ProcessEventHub, IProcessEventHub, WorkflowsContext>
{
    public ProcessCommand(WorkflowsContext db, IHubContext<ProcessEventHub, IProcessEventHub> events)
    : base(db, events)
    { }
}
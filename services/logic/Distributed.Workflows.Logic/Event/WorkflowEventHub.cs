using Distributed.Core.Services;
using Distributed.Workflows.Schema;

namespace Distributed.Workflows.Logic;
public class WorkflowEventHub : EventHub<Workflow, IWorkflowEventHub>
{ }
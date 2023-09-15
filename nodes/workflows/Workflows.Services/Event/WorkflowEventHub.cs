using Distributed.Core.Services;
using Workflows.Entities;

namespace Workflows.Services;
public class WorkflowEventHub : EventHub<Workflow, IWorkflowEventHub>
{ }
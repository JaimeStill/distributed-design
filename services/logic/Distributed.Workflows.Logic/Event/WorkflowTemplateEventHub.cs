using Distributed.Core.Services;
using Distributed.Workflows.Schema;

namespace Distributed.Workflows.Logic;
public class WorkflowTemplateEventHub : EventHub<WorkflowTemplate, IWorkflowTemplateEventHub>
{ }
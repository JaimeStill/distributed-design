using Distributed.Core.Services;
using Workflows.Entities;

namespace Workflows.Services;
public class WorkflowTemplateEventHub : EventHub<WorkflowTemplate, IWorkflowTemplateEventHub>
{ }
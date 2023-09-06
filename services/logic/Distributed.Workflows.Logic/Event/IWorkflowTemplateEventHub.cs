using Distributed.Core.Services;
using Distributed.Workflows.Schema;

namespace Distributed.Workflows.Logic;
public interface IWorkflowTemplateEventHub : IEventHub<WorkflowTemplate>
{ }
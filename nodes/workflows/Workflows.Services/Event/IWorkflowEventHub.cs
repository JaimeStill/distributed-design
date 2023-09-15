using Distributed.Core.Services;
using Workflows.Entities;

namespace Workflows.Services;
public interface IWorkflowEventHub : IEventHub<Workflow>
{ }
using Distributed.Core.Services;
using Distributed.Workflows.Schema;

namespace Distributed.Workflows.Logic;
public interface IWorkflowEventHub : IEventHub<Workflow>
{ }
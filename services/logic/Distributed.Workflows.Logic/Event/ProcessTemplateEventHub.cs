using Distributed.Core.Services;
using Distributed.Workflows.Schema;
namespace Distributed.Workflows.Logic;
public class ProcessTemplateEventHub : EventHub<ProcessTemplate, IProcessTemplateEventHub>
{ }
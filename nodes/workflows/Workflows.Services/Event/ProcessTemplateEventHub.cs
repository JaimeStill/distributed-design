using Distributed.Core.Services;
using Workflows.Entities;

namespace Workflows.Services;
public class ProcessTemplateEventHub : EventHub<ProcessTemplate, IProcessTemplateEventHub>
{ }
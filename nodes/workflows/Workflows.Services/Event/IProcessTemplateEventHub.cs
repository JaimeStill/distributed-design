using Distributed.Core.Services;
using Workflows.Entities;

namespace Workflows.Services;
public interface IProcessTemplateEventHub : IEventHub<ProcessTemplate>
{ }
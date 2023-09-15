using Distributed.Core.Services;
using Workflows.Entities;

namespace Workflows.Services;
public interface IProcessEventHub : IEventHub<Process>
{ }
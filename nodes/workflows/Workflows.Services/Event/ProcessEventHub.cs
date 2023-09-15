using Distributed.Core.Services;
using Workflows.Entities;

namespace Workflows.Services;
public class ProcessEventHub : EventHub<Process, IProcessEventHub>
{ }
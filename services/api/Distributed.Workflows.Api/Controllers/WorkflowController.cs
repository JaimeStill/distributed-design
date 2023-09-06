using Distributed.Core.Controllers;
using Distributed.Workflows.Logic;
using Distributed.Workflows.Schema;
using Microsoft.AspNetCore.Mvc;

namespace Distributed.Workflows.Api.Controllers;

[Route("api/[controller]")]
public class WorkflowController : EntityController<Workflow, WorkflowQuery, WorkflowCommand>
{
    public WorkflowController(WorkflowQuery query, WorkflowCommand command)
    : base(query, command)
    { }
}
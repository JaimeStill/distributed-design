using Distributed.Core.Controllers;
using Microsoft.AspNetCore.Mvc;
using Workflows.Entities;
using Workflows.Services;

namespace Workflows.Api.Controllers;

[Route("api/[controller]")]
public class WorkflowController : EntityController<Workflow, WorkflowQuery, WorkflowCommand>
{
    public WorkflowController(WorkflowQuery query, WorkflowCommand command)
    : base(query, command)
    { }
}
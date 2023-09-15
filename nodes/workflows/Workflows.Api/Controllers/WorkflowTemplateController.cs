using Distributed.Core.Controllers;
using Microsoft.AspNetCore.Mvc;
using Workflows.Entities;
using Workflows.Services;

namespace Workflows.Api.Controllers;

[Route("api/[controller]")]
public class WorkflowTemplateController : EntityController<WorkflowTemplate, WorkflowTemplateQuery, WorkflowTemplateCommand>
{
    public WorkflowTemplateController(WorkflowTemplateQuery query, WorkflowTemplateCommand command)
    : base(query, command)
    { }
}
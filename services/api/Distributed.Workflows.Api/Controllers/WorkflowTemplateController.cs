using Distributed.Core.Controllers;
using Distributed.Workflows.Logic;
using Distributed.Workflows.Schema;
using Microsoft.AspNetCore.Mvc;

namespace Distributed.Workflows.Api.Controllers;

[Route("api/[controller]")]
public class WorkflowTemplateController : EntityController<WorkflowTemplate, WorkflowTemplateQuery, WorkflowTemplateCommand>
{
    public WorkflowTemplateController(WorkflowTemplateQuery query, WorkflowTemplateCommand command)
    : base(query, command)
    { }
}
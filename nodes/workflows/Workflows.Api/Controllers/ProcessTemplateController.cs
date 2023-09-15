using Distributed.Core.Controllers;
using Microsoft.AspNetCore.Mvc;
using Workflows.Entities;
using Workflows.Services;

namespace Workflows.Api.Controllers;

[Route("api/[controller]")]
public class ProcessTemplateController : EntityController<ProcessTemplate, ProcessTemplateQuery, ProcessTemplateCommand>
{
    public ProcessTemplateController(ProcessTemplateQuery query, ProcessTemplateCommand command)
    : base(query, command)
    { }
}
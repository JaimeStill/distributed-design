using Distributed.Core.Controllers;
using Distributed.Workflows.Logic;
using Distributed.Workflows.Schema;
using Microsoft.AspNetCore.Mvc;

namespace Distributed.Workflows.Api.Controllers;

[Route("api/[controller]")]
public class ProcessTemplateController : EntityController<ProcessTemplate, ProcessTemplateQuery, ProcessTemplateCommand>
{
    public ProcessTemplateController(ProcessTemplateQuery query, ProcessTemplateCommand command)
    : base(query, command)
    { }
}
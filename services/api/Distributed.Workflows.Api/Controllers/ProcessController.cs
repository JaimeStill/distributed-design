using Distributed.Core.Controllers;
using Distributed.Workflows.Logic;
using Distributed.Workflows.Schema;
using Microsoft.AspNetCore.Mvc;

namespace Distributed.Workflows.Api.Controllers;

[Route("api/[controller]")]
public class ProcessController : EntityController<Process, ProcessQuery, ProcessCommand>
{
    public ProcessController(ProcessQuery query, ProcessCommand command)
    : base(query, command)
    { }
}
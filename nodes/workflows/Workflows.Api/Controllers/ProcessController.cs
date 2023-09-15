using Distributed.Core.Controllers;
using Microsoft.AspNetCore.Mvc;
using Workflows.Entities;
using Workflows.Services;

namespace Workflows.Api.Controllers;

[Route("api/[controller]")]
public class ProcessController : EntityController<Process, ProcessQuery, ProcessCommand>
{
    public ProcessController(ProcessQuery query, ProcessCommand command)
    : base(query, command)
    { }
}
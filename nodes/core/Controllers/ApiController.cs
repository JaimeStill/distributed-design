using Distributed.Core.Messages;
using Microsoft.AspNetCore.Mvc;

namespace Distributed.Core.Controllers;
public abstract class ApiController : ControllerBase
{
    protected IActionResult GatewayResult<T>(T? data) => Ok(data);

    protected IActionResult ApiResult<T>(T? data) => data switch
    {
        IApiMessage result => HandleApiResult(result),
        ValidationMessage validation => HandleValidation(validation),
        _ => HandleResult(data)
    };

    IActionResult HandleValidation(ValidationMessage validation) =>
        validation.IsValid
            ? Ok(validation)
            : BadRequest(validation.Message);
    
    IActionResult HandleApiResult(IApiMessage result) =>
        result.Error
            ? BadRequest(result.Message)
            : result.HasData
                ? Ok(result)
                : NotFound(result);

    IActionResult HandleResult<T>(T? result) =>
        result is null
            ? NotFound(result)
            : Ok(result);
}
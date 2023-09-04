using Distributed.Core.Schema;
using Distributed.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Distributed.Core.Controllers;
public abstract class EntityController<T,TQuery,TCommand> : ApiController
where T : Entity
where TQuery : IQuery<T>
where TCommand : ICommand<T>
{
    protected IQuery<T> baseQuery;
    protected ICommand<T> baseCommand;

    public EntityController(IQuery<T> query, ICommand<T> command)
    {
        baseQuery = query;
        baseCommand = command;
    }

    [HttpGet("[action]")]
    public virtual async Task<IActionResult> Get() =>
        ApiResult(await baseQuery.Get());

    [HttpGet("[action]/{id:int}")]
    public virtual async Task<IActionResult> GetFromId([FromRoute]int id) =>
        ApiResult(await baseQuery.GetById(id));

    [HttpPost("[action]")]
    public virtual async Task<IActionResult> ValidateValue([FromBody]T entity) =>
        ApiResult(await baseCommand.ValidateValue(entity));

    [HttpPost("[action]")]
    public virtual async Task<IActionResult> Validate([FromBody]T entity) =>
        ApiResult(await baseCommand.Validate(entity));

    [HttpPost("[action]")]
    public virtual async Task<IActionResult> Save([FromBody]T entity) =>
        ApiResult(await baseCommand.Save(entity));

    [HttpPost("[action]")]
    public virtual async Task<IActionResult> Remove([FromBody]T entity) =>
        ApiResult(await baseCommand.Remove(entity));
}
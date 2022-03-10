using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using todos.common.Logic;
using Todos.DTOs.Requests;
using Todos.DTOs.Responses;
using Todos.Models.Entities;
using Todos.Utils.Data;
using Todos.Utils.Query;

namespace Todos.API.Controllers;

// TODO: Implement auth prior to releasing to market
[ApiController]
[Route("todos/lists")]
public class TodoListController : ControllerBase
{
    private readonly ILogger<TodoListController> _logger;
    private readonly IHandler<TodoList, TodoListRequest> _handler;
    public TodoListController(ILogger<TodoListController> logger, IHandler<TodoList, TodoListRequest> handler)
    {
        this._logger = logger;
        this._handler = handler;
    }

    [HttpPost("")]
    public async Task<IActionResult> Post([FromBody] TodoListRequest req)
    {
        try
        {
            var result = this._handler.Create(req);
            var ret = new TodoListResponse(result);
            return new JsonResult(ret);
        }
        catch (ArgumentNullException e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e);
        }
    }
    
    [HttpGet("")]
    public async Task<IActionResult> Get([FromQuery] IQueryOptions query)
    {
        try
        {
            var results = this._handler.Get(query);
            var ret = results.Select(x => new TodoListResponse(x));
            return new JsonResult(ret);
        }
        catch (ArgumentNullException e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e);
        }
    }
}
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
    private readonly IHandler<TodoList, TodoListRequest> _handler;
    public TodoListController(IHandler<TodoList, TodoListRequest> handler)
    {
        this._handler = handler;
    }

    [HttpPost("")]
    public async Task<IActionResult> Post([FromBody] TodoListRequest req)
    {
        try
        {
            var result = this._handler.Create(req);
            var ret = new TodoListResponse(result);
            return StatusCode(StatusCodes.Status201Created, new JsonResult(ret));
        }
        catch (ArgumentNullException e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.ToString());
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.ToString());
        }
    }
    
    [HttpGet("")]
    public async Task<IActionResult> Get([FromQuery] QueryOptions query)
    {
        try
        {
            var results = this._handler.Get(query);
            var ret = results.Select(x => new TodoListResponse(x)).ToList();
            return StatusCode(StatusCodes.Status200OK,  new JsonResult(ret));
        }
        catch (ArgumentNullException e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.ToString());
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.ToString());
        }
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id, [FromQuery] bool includeArchived)
    {
        try
        {
            var results = this._handler.Get(id, includeArchived);
            if (results == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            var ret = new TodoListResponse(results);
            return StatusCode(StatusCodes.Status200OK, new JsonResult(ret));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.ToString());
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Archive([FromRoute] int id)
    {
        try
        {
            var results = this._handler.Archive(id);
            var ret = new TodoListResponse(results);
            return StatusCode(StatusCodes.Status202Accepted,new JsonResult(ret));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.ToString());
        }
    }
    
    [HttpPatch("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] TodoListRequest req)
    {
        try
        {
            var results = this._handler.Update(req, id);
            var ret = new TodoListResponse(results);
            return StatusCode(StatusCodes.Status200OK, new JsonResult(ret));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.ToString());
        }
    }
}
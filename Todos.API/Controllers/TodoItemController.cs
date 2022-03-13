using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using Todos.API.Logic.Handlers;
using todos.common.Logic;
using Todos.DTOs.Requests;
using Todos.DTOs.Responses;
using Todos.Models.Entities;
using Todos.Utils.Data;
using Todos.Utils.Query;

namespace Todos.API.Controllers;

// TODO: Implement auth prior to releasing to market
[ApiController]
[Route("todos/items")]
public class TodoItemController : ControllerBase
{
    private readonly ITodoItemHandler _handler;
    public TodoItemController(ITodoItemHandler handler)
    {
        this._handler = handler;
    }

    [HttpPost("")]
    public async Task<IActionResult> Post([FromBody] TodoItemRequest req)
    {
        try
        {
            var result = this._handler.Create(req);
            var ret = new TodoItemResponse(result);
            return StatusCode(StatusCodes.Status201Created, new JsonResult(ret));
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
    public async Task<IActionResult> Get([FromQuery] TodoItemQueryOptions query)
    {
        try
        {
            var results = this._handler.Get(query);
            var ret = results.Select(x => new TodoItemResponse(x)).ToList();
            return StatusCode(StatusCodes.Status200OK,  new JsonResult(ret));
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
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id, [FromQuery] bool includeArchived)
    {
        try
        {
            var results = this._handler.Get(id, includeArchived);
            var ret = new TodoItemResponse(results);
            return StatusCode(StatusCodes.Status200OK, new JsonResult(ret));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e);
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Archive([FromRoute] int id)
    {
        try
        {
            var results = this._handler.Archive(id);
            var ret = new TodoItemResponse(results);
            return StatusCode(StatusCodes.Status202Accepted,new JsonResult(ret));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e);
        }
    }
    
    [HttpPatch("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] TodoItemRequest req)
    {
        try
        {
            var results = this._handler.Update(req, id);
            var ret = new TodoItemResponse(results);
            return StatusCode(StatusCodes.Status200OK, new JsonResult(ret));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e);
        }
    }
    
    [HttpPatch("{id}/complete")] // TODO: TEST
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CompleteTodoItemRequest req)
    {
        try
        {
            var results = this._handler.Update(req, id);
            var ret = new TodoItemResponse(results);
            return StatusCode(StatusCodes.Status200OK, new JsonResult(ret));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e);
        }
    }
}
using todos.common.Logic;
using Todos.DTOs.Requests;
using Todos.Models.Entities;

namespace Todos.API.Logic.Handlers;

public interface ITodoItemHandler : IHandler<TodoItem, TodoItemRequest>
{
    TodoItem Update(CompleteTodoItemRequest req, int id);
}
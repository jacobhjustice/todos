using Todos.DTOs.Requests;
using Todos.Models.Entities;
using Todos.Utils.Data;

namespace Todos.Repositories;

public interface IReadOnlyTodoItemRepository: IReadOnlyRepository<TodoItem>
{
    IQueryable<TodoItem> GetAll(TodoItemQueryOptions? options);
    TodoItem? Get(string label);
}
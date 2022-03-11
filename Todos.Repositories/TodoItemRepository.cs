using Microsoft.EntityFrameworkCore;
using Todos.DTOs.Requests;
using Todos.Models;
using Todos.Models.Entities;
using Todos.Utils.Data;
using Todos.Utils.Query;

namespace Todos.Repositories;

public class TodoItemRepository : ReadWriteRepository<TodoItem>, IReadOnlyTodoItemRepository
{
    public TodoItemRepository(TodoContext context) : base(context) {}

    private IQueryable<TodoItem> FilterByTodoListId(IQueryable<TodoItem> query, int todoListId) =>
        query
            .Where(x => x.TodoListId == todoListId);
    public IQueryable<TodoItem> GetAll(TodoItemQueryOptions? options)
    {
        var query = base.GetAll(options);
        if (options.TodoListId.HasValue)
        {
            query = this.FilterByTodoListId(query, options.TodoListId.Value);
        }

        return query;
    }
    
}
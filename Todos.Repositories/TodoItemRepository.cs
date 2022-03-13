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
    
    private IQueryable<TodoItem> FilterByTodoCompleted(IQueryable<TodoItem> query, bool isComplete) =>
        query
            .Where(x => isComplete ? x.CompletedAt != null : x.CompletedAt == null);
    
    private IQueryable<TodoItem> FilterByLabel(IQueryable<TodoItem> query, string label) =>
        query
            .Where(x => x.Label == label);
    
    public IQueryable<TodoItem> GetAll(TodoItemQueryOptions? options)
    {
        var query = base.GetAll(options);
        if (options != null)
        {
            if (options.TodoListId.HasValue)
            {
                query = this.FilterByTodoListId(query, options.TodoListId.Value);
            }
        
            if (options.Completed.HasValue)
            {
                query = this.FilterByTodoCompleted(query, options.Completed.Value);
            }
        }

        return query;
    }
    
    public TodoItem? Get(string label)
    {
        var query = this.GetAll(null);
        query = this.FilterByLabel(query, label);
        return query.FirstOrDefault();
    }
}
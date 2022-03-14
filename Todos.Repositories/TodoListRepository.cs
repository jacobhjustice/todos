using Microsoft.EntityFrameworkCore;
using Todos.Models;
using Todos.Models.Entities;
using Todos.Utils.Data;

namespace Todos.Repositories;

public class TodoListRepository : ReadWriteRepository<TodoList>, IReadOnlyTodoListRepository
{
    public TodoListRepository(TodoContext context) : base(context) {}

    private IQueryable<TodoList> FilterByLabel(IQueryable<TodoList> query, string label) =>
        query
            .Where(x => x.Label == label);
    public TodoList? Get(string label)
    {
        var query = this.GetAll(null);
        query = this.FilterByLabel(query, label);
        return query.FirstOrDefault();
    }
}
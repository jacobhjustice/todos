using Microsoft.EntityFrameworkCore;
using Todos.Models;
using Todos.Models.Entities;
using Todos.Utils.Data;

namespace Todos.Repositories;

public class TodoListRepository : ReadWriteRepository<TodoList>, IReadOnlyTodoListRepository
{
    public TodoListRepository(TodoContext context) : base(context) {}

    public TodoList? Get(string label)
    {
        return this.GetAll(null)
            .FirstOrDefault(x => x.Label == label);
    }
}
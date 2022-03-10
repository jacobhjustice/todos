using Microsoft.EntityFrameworkCore;
using Todos.Models;
using Todos.Models.Entities;
using Todos.Utils.Data;

namespace Todos.Repositories;

public class TodoListRepository : ReadWriteRepository<TodoList>
{
    public TodoListRepository(TodoContext context) : base(context) {}
}
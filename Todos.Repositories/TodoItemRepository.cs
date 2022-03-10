using Microsoft.EntityFrameworkCore;
using Todos.Models;
using Todos.Models.Entities;
using Todos.Utils.Data;

namespace Todos.Repositories;

public class TodoItemRepository : ReadWriteRepository<TodoItem>
{
    public TodoItemRepository(TodoContext context) : base(context) {}
}
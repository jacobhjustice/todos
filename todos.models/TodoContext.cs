using Microsoft.EntityFrameworkCore;

namespace todos.models;

public class TodoContext : DbContext
{
    public DbSet<TodoList> TodoLists { get; set; }
    public DbSet<TodoItems> Assignments { get; set; }

}
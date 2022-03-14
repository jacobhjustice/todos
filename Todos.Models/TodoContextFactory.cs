
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace Todos.Models;

public class TodoContextFactory : IDbContextFactory<TodoContext>, IDesignTimeDbContextFactory<TodoContext>
{
    public TodoContext CreateDbContext()
    {
        return new TodoContext("=");
    }

    public TodoContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TodoContext>();
        var connection = args[0];
        optionsBuilder.UseSqlServer(connection);

        return new TodoContext(optionsBuilder.Options);
    }
}
// dotnet ef migrations add Todos_Baseline --project Todos.Models -- "Data Source=localhost:1401;Initial Catalog=Todos;User ID=SA;Password=PASS-WORD_123;"


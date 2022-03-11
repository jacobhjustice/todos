using Microsoft.EntityFrameworkCore;
using Todos.Models.Entities;

namespace Todos.Models;

public class TodoContext : DbContext
{
    public DbSet<TodoList> TodoLists { get; set; }
    public DbSet<TodoItem> TodoItems { get; set; }


    public TodoContext(string connString) : base(GetOptions(connString))
    {
        this.RunMigrations();
    }

    // For local testing purposes only
    private void RunMigrations()
    {
        if (this.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
        {
            this.Database.Migrate();
        }
    }


    private static DbContextOptions GetOptions(string connectionString)
    {
        return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString)
            .Options;
    }

    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options)
    {
        this.RunMigrations();
    }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //     => optionsBuilder
    //         .UseLazyLoadingProxies()
    //         .ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.DetachedLazyLoadingWarning));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<TodoItem>()
            .ToTable("TodoItems")
            .HasKey(x => x.Id);

        modelBuilder.Entity<TodoItem>()
            .ToTable("TodoItems")
            .HasKey(x => x.Id);

        modelBuilder.Entity<TodoItem>()
            .HasOne(x => x.TodoList)
            .WithMany(x => x.TodoItems)
            .HasForeignKey(x => x.TodoListId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }

}
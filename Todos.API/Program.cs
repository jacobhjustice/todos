using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Todos.API.Logic.Handlers;
using todos.common.Logic;
using Todos.DTOs.Requests;
using Todos.Models;
using Todos.Models.Entities;
using Todos.Repositories;
using Todos.Utils.Data;
using Todos.Validations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<TodoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));
builder.Services.AddControllers();

builder.Services.AddTransient<IValidator<TodoList>, TodoListValidator>();
builder.Services.AddTransient<IValidator<TodoItem>, TodoItemValidator>();

builder.Services.AddTransient<IHandler<TodoList, TodoListRequest>, TodoListHandler>();
builder.Services.AddTransient<IHandler<TodoItem, TodoItemRequest>, TodoItemHandler>();
builder.Services.AddTransient<ITodoItemHandler, TodoItemHandler>();

builder.Services.AddTransient<IReadOnlyRepository<TodoItem>, TodoItemRepository>();
builder.Services.AddTransient<IReadOnlyTodoItemRepository, TodoItemRepository>();
builder.Services.AddTransient<IWriteOnlyRepository<TodoItem>, TodoItemRepository>();

builder.Services.AddTransient<IReadOnlyRepository<TodoList>, TodoListRepository>();
builder.Services.AddTransient<IReadOnlyTodoListRepository, TodoListRepository>();
builder.Services.AddTransient<IWriteOnlyRepository<TodoList>, TodoListRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// app.UseAuthorization();

app.MapControllers();

app.Run();
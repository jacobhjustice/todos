using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Todos.DTOs.Requests;
using Todos.Models;
using Todos.Models.Entities;
using Todos.Utils.Data;
using Todos.Utils.Query;
using Xunit;

namespace Todos.Repositories.Tests;

public class TodoItemRepositoryTests
{
    

    private (TodoItemRepository, TodoContext) SetupData()
    {
        var records = new List<TodoItem>();
        records.Add(new TodoItem{Id = 1, CreatedAt = DateTime.Now.AddDays(-6), Label = "1", ArchivedAt = null, TodoListId = 123});
        records.Add(new TodoItem{Id = 2, CreatedAt = DateTime.Now.AddDays(-6), Label = "2", ArchivedAt = DateTime.Now.AddDays(-2), CompletedAt = DateTime.Now, TodoListId = 2});
        records.Add(new TodoItem{Id = 3, CreatedAt = DateTime.Now.AddDays(-2), Label = "3", ArchivedAt = DateTime.Now.AddDays(-6), TodoListId = 123});
        records.Add(new TodoItem{Id = 5, CreatedAt = DateTime.Now.AddDays(-1), Label = "4", ArchivedAt = null, CompletedAt = DateTime.Now, TodoListId = 1});
        records.Add(new TodoItem{Id = 4, CreatedAt = DateTime.Now.AddDays(-10), Label = "5", ArchivedAt = DateTime.Now, CompletedAt = DateTime.Now, TodoListId = 5});


        var options = new DbContextOptionsBuilder<TodoContext>()
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging(true)
            .Options;

        var context = new TodoContext(options);

        foreach (var sr in records)
        {
            context.Add(sr);
            context.SaveChanges();
        }


        return (new TodoItemRepository(context), context);
    }
    
    [Theory]
    [InlineData(null, null, null, false, true, null, null, new int[] {1,2,3,5,4})]
    [InlineData(2, null, null, false, true, null, null, new int[] {1,2})]
    [InlineData(1, 2, null, false, true, null, null, new int[] {3})]
    [InlineData(null, null, "Id", false, true, null, null, new int[] {1,2,3,4,5})]
    [InlineData(null, null, "Id", true, true, null, null, new int[] {5,4,3,2,1})]
    [InlineData(null, null, "ArchivedAt", false, true, null, null, new int[] {1,5,3,2,4})]
    [InlineData(null, null, null, false, false, null, null, new int[] {1,5})]
    [InlineData(null, null, null, false, true, true, null, new int[] {2,5,4})]
    [InlineData(null, null, null, false, true, false, null, new int[] {1,3})]
    [InlineData(null, null, null, false, true, null, 123, new int[] {1,3})]
    public void GetAll(int? limit, int? offset, string? order, bool isDescending, bool includeArchived, bool? isCompleted, int? listId, int[] expectedIds)
    {
        var query = new TodoItemQueryOptions
        {
            Limit = limit,
            Offset = offset,
            Order = order,
            IsDescending = isDescending,
            IncludeArchived = includeArchived,
            Completed = isCompleted,
            TodoListId = listId
        };
        var repo = this.SetupData().Item1;
        var results = repo.GetAll(query).ToList();
        Assert.InRange(results.Count, expectedIds.Length, expectedIds.Length);
        for (var i = 0; i < expectedIds.Length; i++)
        {
            Assert.Equal(expectedIds[i], results[i].Id);
        }
    }
}
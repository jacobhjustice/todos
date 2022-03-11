using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Todos.Models;
using Todos.Models.Entities;
using Todos.Utils.Data;
using Xunit;

namespace Todos.Repositories.Tests;

public class UnitTest1
{
    private (TodoListRepository, TodoContext) SetupData()
    {
        var records = new List<TodoList>();
        records.Add(new TodoList{Id = 1, Label = "test"});
        records.Add(new TodoList{Id = 2, Label = "blub"});


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


        return (new TodoListRepository(context), context);
    }
    
    [Theory]
    [InlineData("test", 1)]
    [InlineData("blub", 2)]
    [InlineData("derp", null)]

    public void Get_ByLabel(string lookupLabel, int? expectedId)
    {
        var (repo, ctx) = this.SetupData();
        var result = repo.Get(lookupLabel);
        if (expectedId.HasValue)
        {
            Assert.NotNull(result);
            Assert.Equal(expectedId, result.Id);
        }
        else
        {
            Assert.Null(result);
        }
    }
}
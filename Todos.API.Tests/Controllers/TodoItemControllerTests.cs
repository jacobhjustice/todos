using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Todos.API.Controllers;
using Todos.API.Logic.Handlers;
using todos.common.Logic;
using Todos.DTOs.Requests;
using Todos.DTOs.Responses;
using Todos.Models.Entities;
using Todos.Repositories;
using Todos.Utils.Data;
using Todos.Utils.Query;
using Xunit;

namespace Todos.API.Tests.Controllers;

public class TodoItemControllerTests
{
    private readonly Mock<ITodoItemHandler> _handler;

    public TodoItemControllerTests()
    {
        this._handler = new Mock<ITodoItemHandler>();
    }
    
    private TodoItemController _controller => new TodoItemController(this._handler.Object);

    [Theory]
    [InlineData(1, 3, "test", false, true)]
    [InlineData(2, 4,  "blub", true, true)]
    [InlineData(1, 5, "test", false, false)]
    [InlineData(2, 2,  "blub", true, false)]
    public async void Post_Success(int id, int listId, string label, bool isArchived, bool isCompleted)
    {
        this._handler.Setup(x => x.Create(It.IsAny<TodoItemRequest>())).Returns(new TodoItem
        {
            Id = id,
            Label = label,
            ArchivedAt = isArchived ? DateTime.Now : null,
            CompletedAt = isCompleted ? DateTime.Now : null,
            TodoListId = listId
        });

        var result = await this._controller.Post(new TodoItemRequest());
        var status = result as ObjectResult;
        Assert.NotNull(status);
        Assert.Equal(201, status.StatusCode);
        var json = status.Value as JsonResult;
        Assert.NotNull(json);
        var response = json.Value as TodoItemResponse;
        
        Assert.NotNull(response);
        Assert.Equal(label, response.Label);
        Assert.Equal(id, response.Id);
        Assert.Equal(isArchived, response.IsArchived);
        Assert.Equal(isCompleted, response.IsCompleted);
        Assert.Equal(listId, response.TodoListId);

    }
    
    [Theory]
    [InlineData(1, 3, "test", false, true)]
    [InlineData(2, 4,  "blub", true, true)]
    [InlineData(1, 5, "test", false, false)]
    [InlineData(2, 2,  "blub", true, false)]
    public async void Update_Success(int id, int listId, string label, bool isArchived, bool isCompleted)
    {
        this._handler.Setup(x => x.Update(It.IsAny<TodoItemRequest>(), 1)).Returns(new TodoItem()
        {
            Id = id,
            Label = label,
            ArchivedAt = isArchived ? DateTime.Now : null,
            CompletedAt = isCompleted ? DateTime.Now : null,
            TodoListId = listId
        });
    
        var result = await this._controller.Update(1, new TodoItemRequest());
        var status = result as ObjectResult;
        Assert.NotNull(status);
        Assert.Equal(200, status.StatusCode);
        var json = status.Value as JsonResult;
        Assert.NotNull(json);
        var response = json.Value as TodoItemResponse;
        
        Assert.NotNull(response);
        Assert.Equal(label, response.Label);
        Assert.Equal(id, response.Id);
        Assert.Equal(isArchived, response.IsArchived);
        Assert.Equal(isCompleted, response.IsCompleted);
        Assert.Equal(listId, response.TodoListId);
    }
    
    [Theory]
    [InlineData(1, 3, "test", false, true)]
    [InlineData(2, 4,  "blub", true, true)]
    [InlineData(1, 5, "test", false, false)]
    [InlineData(2, 2,  "blub", true, false)]
    public async void Update_Completed_Success(int id, int listId, string label, bool isArchived, bool isCompleted)
    {
        this._handler.Setup(x => x.Update(It.IsAny<CompleteTodoItemRequest>(), 1)).Returns(new TodoItem()
        {
            Id = id,
            Label = label,
            ArchivedAt = isArchived ? DateTime.Now : null,
            CompletedAt = isCompleted ? DateTime.Now : null,
            TodoListId = listId
        });
    
        var result = await this._controller.Update(1, new CompleteTodoItemRequest());
        var status = result as ObjectResult;
        Assert.NotNull(status);
        Assert.Equal(200, status.StatusCode);
        var json = status.Value as JsonResult;
        Assert.NotNull(json);
        var response = json.Value as TodoItemResponse;
        
        Assert.NotNull(response);
        Assert.Equal(label, response.Label);
        Assert.Equal(id, response.Id);
        Assert.Equal(isArchived, response.IsArchived);
        Assert.Equal(isCompleted, response.IsCompleted);
        Assert.Equal(listId, response.TodoListId);
    }
    
    [Theory]
    [InlineData(1, 3, "test", false, true)]
    [InlineData(2, 4,  "blub", true, true)]
    [InlineData(1, 5, "test", false, false)]
    [InlineData(2, 2,  "blub", true, false)]
    public async void Archive_Success(int id, int listId, string label, bool isArchived, bool isCompleted)
    {
        this._handler.Setup(x => x.Archive(1)).Returns(new TodoItem()
        {
            Id = id,
            Label = label,
            ArchivedAt = isArchived ? DateTime.Now : null,
            CompletedAt = isCompleted ? DateTime.Now : null,
            TodoListId = listId
        });
    
        var result = await this._controller.Archive(1);
        var status = result as ObjectResult;
        Assert.NotNull(status);
        Assert.Equal(202, status.StatusCode);
        var json = status.Value as JsonResult;
        Assert.NotNull(json);
        var response = json.Value as TodoItemResponse;
        
        Assert.NotNull(response);
        Assert.Equal(label, response.Label);
        Assert.Equal(id, response.Id);
        Assert.Equal(isArchived, response.IsArchived);
        Assert.Equal(isCompleted, response.IsCompleted);
        Assert.Equal(listId, response.TodoListId);
    }
    
    [Theory]
    [InlineData(1, 3, "test", false, true)]
    [InlineData(2, 4,  "blub", true, true)]
    [InlineData(1, 5, "test", false, false)]
    [InlineData(2, 2,  "blub", true, false)]
    public async void Get_Single_Success(int id, int listId, string label, bool isArchived, bool isCompleted)
    {
        this._handler.Setup(x => x.Get(1, true)).Returns(new TodoItem()
        {
            Id = id,
            Label = label,
            ArchivedAt = isArchived ? DateTime.Now : null,
            CompletedAt = isCompleted ? DateTime.Now : null,
            TodoListId = listId
        });

        var result = await this._controller.Get(1, true);
        var status = result as ObjectResult;
        Assert.NotNull(status);
        Assert.Equal(200, status.StatusCode);
        var json = status.Value as JsonResult;
        Assert.NotNull(json);
        var response = json.Value as TodoItemResponse;
        
        Assert.NotNull(response);
        Assert.Equal(label, response.Label);
        Assert.Equal(id, response.Id);
        Assert.Equal(isArchived, response.IsArchived);
        Assert.Equal(isCompleted, response.IsCompleted);
        Assert.Equal(listId, response.TodoListId);
    }
    
    [Fact]
    public async void Get_Query_Success()
    {
        this._handler.Setup(x => x.Get(It.IsAny<QueryOptions>())).Returns(new List<TodoItem>
        {
            new TodoItem
            {
                Id = 1,
                ArchivedAt = new DateTime(),
                Label = "test1"
            },
            new TodoItem
            {
                Id = 2,
                ArchivedAt = null,
                Label = "test2"
            }
        });
    
        var result = await this._controller.Get(new TodoItemQueryOptions());
        var status = result as ObjectResult;
        Assert.NotNull(status);
        Assert.Equal(200, status.StatusCode);
        var json = status.Value as JsonResult;
        Assert.NotNull(json);
        var response = json.Value as List<TodoItemResponse>;
        
        Assert.NotNull(response);
        Assert.InRange(response.Count, 2, 2);
        Assert.Equal("test1", response[0].Label);
        Assert.Equal("test2", response[1].Label);
        Assert.Equal(1, response[0].Id);
        Assert.Equal(2, response[1].Id);
        Assert.Equal(true, response[0].IsArchived);
        Assert.Equal(false, response[1].IsArchived);
    }
}
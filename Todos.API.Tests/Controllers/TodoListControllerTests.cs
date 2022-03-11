using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Todos.API.Controllers;
using todos.common.Logic;
using Todos.DTOs.Requests;
using Todos.DTOs.Responses;
using Todos.Models.Entities;
using Todos.Repositories;
using Todos.Utils.Data;
using Todos.Utils.Query;
using Xunit;

namespace Todos.API.Tests.Controllers;

public class TodoListControllerTests
{
    private readonly Mock<IHandler<TodoList, TodoListRequest>> _handler;

    public TodoListControllerTests()
    {
        this._handler = new Mock<IHandler<TodoList, TodoListRequest>>();
    }
    
    private TodoListController _controller => new TodoListController(this._handler.Object);

    [Theory]
    [InlineData(1, "test", false)]
    [InlineData(2, "blub", true)]

    public async void Post_Success(int id, string label, bool isArchived)
    {
        this._handler.Setup(x => x.Create(It.IsAny<TodoListRequest>())).Returns(new TodoList
        {
            Id = id,
            Label = label,
            ArchivedAt = isArchived ? DateTime.Now : null
        });

        var result = await this._controller.Post(new TodoListRequest());
        var status = result as ObjectResult;
        Assert.NotNull(status);
        Assert.Equal(201, status.StatusCode);
        var json = status.Value as JsonResult;
        Assert.NotNull(json);
        var response = json.Value as TodoListResponse;
        
        Assert.NotNull(response);
        Assert.Equal(label, response.Label);
        Assert.Equal(id, response.Id);
        Assert.Equal(isArchived, response.IsArchived);
    }
    
    [Theory]
    [InlineData(1, "test", false)]
    [InlineData(2, "blub", true)]

    public async void Update_Success(int id, string label, bool isArchived)
    {
        this._handler.Setup(x => x.Update(It.IsAny<TodoListRequest>(), 1)).Returns(new TodoList
        {
            Id = id,
            Label = label,
            ArchivedAt = isArchived ? DateTime.Now : null
        });

        var result = await this._controller.Update(1, new TodoListRequest());
        var status = result as ObjectResult;
        Assert.NotNull(status);
        Assert.Equal(200, status.StatusCode);
        var json = status.Value as JsonResult;
        Assert.NotNull(json);
        var response = json.Value as TodoListResponse;
        
        Assert.NotNull(response);
        Assert.Equal(label, response.Label);
        Assert.Equal(id, response.Id);
        Assert.Equal(isArchived, response.IsArchived);
    }
    
    [Theory]
    [InlineData(1, "test", false)]
    [InlineData(2, "blub", true)]

    public async void Archive_Success(int id, string label, bool isArchived)
    {
        this._handler.Setup(x => x.Archive(1)).Returns(new TodoList
        {
            Id = id,
            Label = label,
            ArchivedAt = isArchived ? DateTime.Now : null
        });

        var result = await this._controller.Archive(1);
        var status = result as ObjectResult;
        Assert.NotNull(status);
        Assert.Equal(202, status.StatusCode);
        var json = status.Value as JsonResult;
        Assert.NotNull(json);
        var response = json.Value as TodoListResponse;
        
        Assert.NotNull(response);
        Assert.Equal(label, response.Label);
        Assert.Equal(id, response.Id);
        Assert.Equal(isArchived, response.IsArchived);
    }
    
    [Theory]
    [InlineData(1, "test", false)]
    [InlineData(2, "blub", true)]
    public async void Get_Single_Success(int id, string label, bool isArchived)
    {
        this._handler.Setup(x => x.Get(1, true)).Returns(new TodoList
        {
            Id = id,
            Label = label,
            ArchivedAt = isArchived ? DateTime.Now : null
        });

        var result = await this._controller.Get(1, true);
        var status = result as ObjectResult;
        Assert.NotNull(status);
        Assert.Equal(200, status.StatusCode);
        var json = status.Value as JsonResult;
        Assert.NotNull(json);
        var response = json.Value as TodoListResponse;
        
        Assert.NotNull(response);
        Assert.Equal(label, response.Label);
        Assert.Equal(id, response.Id);
        Assert.Equal(isArchived, response.IsArchived);
    }
    
    [Fact]
    public async void Get_Query_Success()
    {
        this._handler.Setup(x => x.Get(It.IsAny<QueryOptions>())).Returns(new List<TodoList>
        {
            new TodoList
            {
                Id = 1,
                ArchivedAt = new DateTime(),
                Label = "test1"
            },
            new TodoList
            {
                Id = 2,
                ArchivedAt = null,
                Label = "test2"
            }
        });

        var result = await this._controller.Get(new QueryOptions());
        var status = result as ObjectResult;
        Assert.NotNull(status);
        Assert.Equal(200, status.StatusCode);
        var json = status.Value as JsonResult;
        Assert.NotNull(json);
        var response = json.Value as List<TodoListResponse>;
        
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
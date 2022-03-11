using System;
using System.Collections.Generic;
using FluentValidation;
using Moq;
using Todos.Models.Entities;
using Todos.Repositories;
using Todos.Utils.Validation;
using Xunit;

namespace Todos.Validations.Tests;

public class TodoListValidatorTests
{
    private readonly Mock<IReadOnlyTodoListRepository> _repository;
    
    public TodoListValidatorTests()
    {
        this._repository = new Mock<IReadOnlyTodoListRepository>();
    }

    private TodoListValidator _validator => new TodoListValidator(this._repository.Object);
    
    [Fact]
    public void Validate_Create_Success()
    {
        this._repository.Setup(x => x.Get(It.IsAny<string>()))
            .Returns<TodoList>(null);

        var result = this._validator.Validate(new TodoList{ Label = "Blub", Id = 3}, options => options.IncludeRuleSets(Rulesets.CREATE));
        
        Assert.True(result.IsValid);
        this._repository.Verify(x => x.Get("Blub"), Times.Once());
    }
    
    [Fact]
    public void Validate_Create_Failure_DuplicateLabel()
    {
        this._repository.Setup(x => x.Get(It.IsAny<string>()))
            .Returns(new TodoList());

        var result = this._validator.Validate(new TodoList{ Label = "Blub", Id = 3}, options => options.IncludeRuleSets(Rulesets.CREATE));
        
        Assert.False(result.IsValid);
        Assert.InRange(result.Errors.Count, 1, 1);
        Assert.Equal("each TodoList label must be unique", result.Errors[0].ErrorMessage);
        this._repository.Verify(x => x.Get("Blub"), Times.Once());
    }
    
    [Fact]
    public void Validate_Update_Success()
    {
        this._repository.Setup(x => x.Get(It.IsAny<string>()))
            .Returns<TodoList>(null);

        this._repository.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns(new TodoList());
        
        var result = this._validator.Validate(new TodoList{ Label = "Blub", Id = 3}, options => options.IncludeRuleSets(Rulesets.UPDATE));
        
        Assert.True(result.IsValid);
        this._repository.Verify(x => x.Get("Blub"), Times.Once());
    }
    
    [Fact]
    public void Validate_Update_Failure_DuplicateLabel()
    {
        this._repository.Setup(x => x.Get(It.IsAny<string>()))
            .Returns(new TodoList());
        
        this._repository.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns(new TodoList());

        var result = this._validator.Validate(new TodoList{ Label = "Blub", Id = 3}, options => options.IncludeRuleSets(Rulesets.UPDATE));
        
        Assert.False(result.IsValid);
        Assert.InRange(result.Errors.Count, 1, 1);
        Assert.Equal("each TodoList label must be unique", result.Errors[0].ErrorMessage);
        this._repository.Verify(x => x.Get("Blub"), Times.Once());
    }
    
    [Fact]
    public void Validate_Update_Failure_IdNotFound()
    {
        this._repository.Setup(x => x.Get(It.IsAny<string>()))
            .Returns<TodoList>(null);
        
        var result = this._validator.Validate(new TodoList{ Label = "Blub", Id = 3}, options => options.IncludeRuleSets(Rulesets.UPDATE));
        
        Assert.False(result.IsValid);
        Assert.InRange(result.Errors.Count, 1, 1);
        Assert.Equal("id must reference an actual TodoList", result.Errors[0].ErrorMessage);
        this._repository.Verify(x => x.Get("Blub"), Times.Once());
    }
    
    [Fact]
    public void Validate_Archive_Success_Null()
    {
        this._repository.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<bool>()))
            .Returns<TodoList>(null);
    
        this._repository.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns(new TodoList());
        
        var result = this._validator.Validate(new TodoList{ Label = "Blub", Id = 3}, options => options.IncludeRuleSets(Rulesets.ARCHIVE));
        
        Assert.True(result.IsValid);
        this._repository.Verify(x => x.Get(3, true));
    }
    
    [Fact]
    public void Validate_Archive_Success_NotArchived()
    {
        this._repository.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<bool>()))
            .Returns(new TodoList{ArchivedAt = null});
    
        this._repository.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns(new TodoList());

        var result = this._validator.Validate(new TodoList{ Label = "Blub", Id = 3}, options => options.IncludeRuleSets(Rulesets.ARCHIVE));
        
        Assert.True(result.IsValid);
        this._repository.Verify(x => x.Get(3, true));
    }
    
    [Fact]
    public void Validate_Archive_Failure_AlreadyArchived()
    {
        this._repository.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<bool>()))
            .Returns(new TodoList{ArchivedAt = new DateTime()});
    
        var result = this._validator.Validate(new TodoList{ Label = "Blub", Id = 3}, options => options.IncludeRuleSets(Rulesets.ARCHIVE));

        Assert.False(result.IsValid);
        Assert.InRange(result.Errors.Count, 1, 1);
        Assert.Equal("cannot archive a TodoList that is already archived", result.Errors[0].ErrorMessage);
        this._repository.Verify(x => x.Get(3, true));
    }
    
    [Fact]
    public void Validate_Archive_Failure_NotFound()
    {
        this._repository.Setup(x => x.Get(It.IsAny<int>(), true))
            .Returns<TodoList>(null);

        var result = this._validator.Validate(new TodoList{ Label = "Blub", Id = 3}, options => options.IncludeRuleSets(Rulesets.ARCHIVE));

        Assert.False(result.IsValid);
        Assert.InRange(result.Errors.Count, 1, 1);
        Assert.Equal("id must reference an actual TodoList", result.Errors[0].ErrorMessage);
        this._repository.Verify(x => x.Get(3, true));
    }
}
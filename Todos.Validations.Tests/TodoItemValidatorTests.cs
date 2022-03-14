using System;
using System.Collections.Generic;
using FluentValidation;
using Moq;
using Todos.Models.Entities;
using Todos.Repositories;
using Todos.Utils.Validation;
using Xunit;

namespace Todos.Validations.Tests;

public class TodoItemValidatorTests
{
    private readonly Mock<IReadOnlyTodoItemRepository> _repository;
    
    public TodoItemValidatorTests()
    {
        this._repository = new Mock<IReadOnlyTodoItemRepository>();
    }

    private TodoItemValidator _validator => new TodoItemValidator(this._repository.Object);
    
    [Fact]
    public void Validate_Create_Success()
    {
        this._repository.Setup(x => x.Get(It.IsAny<string>()))
            .Returns<TodoItem>(null);

        var result = this._validator.Validate(new TodoItem{ Label = "Blub", Id = 3}, options => options.IncludeRuleSets(Rulesets.CREATE));
        
        Assert.True(result.IsValid);
        this._repository.Verify(x => x.Get("Blub"), Times.Once());
    }
    
    [Fact]
    public void Validate_Create_Failure_DuplicateLabel()
    {
        this._repository.Setup(x => x.Get(It.IsAny<string>()))
            .Returns(new TodoItem());

        var result = this._validator.Validate(new TodoItem{ Label = "Blub", Id = 3}, options => options.IncludeRuleSets(Rulesets.CREATE));
        
        Assert.False(result.IsValid);
        Assert.InRange(result.Errors.Count, 1, 1);
        Assert.Equal("each TodoItem label must be unique", result.Errors[0].ErrorMessage);
        this._repository.Verify(x => x.Get("Blub"), Times.Once());
    }
    
    [Fact]
    public void Validate_Update_Success()
    {
        this._repository.Setup(x => x.Get(It.IsAny<string>()))
            .Returns<TodoItem>(null);

        this._repository.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns(new TodoItem());
        
        var result = this._validator.Validate(new TodoItem{ Label = "Blub", Id = 3}, options => options.IncludeRuleSets(Rulesets.UPDATE));
        
        Assert.True(result.IsValid);
        this._repository.Verify(x => x.Get("Blub"), Times.Once());
    }
    
    [Fact]
    public void Validate_Update_Failure_DuplicateLabel()
    {
        this._repository.Setup(x => x.Get(It.IsAny<string>()))
            .Returns(new TodoItem());
        
        this._repository.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns(new TodoItem());

        var result = this._validator.Validate(new TodoItem{ Label = "Blub", Id = 3}, options => options.IncludeRuleSets(Rulesets.UPDATE));
        
        Assert.False(result.IsValid);
        Assert.InRange(result.Errors.Count, 1, 1);
        Assert.Equal("each TodoItem label must be unique", result.Errors[0].ErrorMessage);
        this._repository.Verify(x => x.Get("Blub"), Times.Once());
    }
    
    [Fact]
    public void Validate_Update_Failure_IdNotFound()
    {
        this._repository.Setup(x => x.Get(It.IsAny<string>()))
            .Returns<TodoItem>(null);
        
        var result = this._validator.Validate(new TodoItem{ Label = "Blub", Id = 3}, options => options.IncludeRuleSets(Rulesets.UPDATE));
        
        Assert.False(result.IsValid);
        Assert.InRange(result.Errors.Count, 1, 1);
        Assert.Equal("TodoItem must exist and not be archived", result.Errors[0].ErrorMessage);
        this._repository.Verify(x => x.Get("Blub"), Times.Once());
    }
    
    [Fact]
    public void Validate_Archive_Success_Null()
    {
        this._repository.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<bool>()))
            .Returns<TodoItem>(null);
    
        this._repository.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns(new TodoItem());
        
        var result = this._validator.Validate(new TodoItem{ Label = "Blub", Id = 3}, options => options.IncludeRuleSets(Rulesets.ARCHIVE));
        
        Assert.True(result.IsValid);
    }
    
    [Fact]
    public void Validate_Archive_Success_NotArchived()
    {
        this._repository.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<bool>()))
            .Returns(new TodoItem{ArchivedAt = null});
    
        this._repository.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns(new TodoItem());

        var result = this._validator.Validate(new TodoItem{ Label = "Blub", Id = 3}, options => options.IncludeRuleSets(Rulesets.ARCHIVE));
        
        Assert.True(result.IsValid);
    }
    
    [Fact]
    public void Validate_Archive_Failure_AlreadyArchived()
    {
        this._repository.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<bool>()))
            .Returns<TodoItem>(null);
    
        var result = this._validator.Validate(new TodoItem{ Label = "Blub", Id = 3}, options => options.IncludeRuleSets(Rulesets.ARCHIVE));

        Assert.False(result.IsValid);
        Assert.InRange(result.Errors.Count, 1, 1);
        Assert.Equal("TodoItem must exist and not be archived", result.Errors[0].ErrorMessage);
    }
    
    [Fact]
    public void Validate_Failure_EmptyLabel()
    {
        this._repository.Setup(x => x.Get(It.IsAny<string>()))
            .Returns<TodoItem>(null);

        var result = this._validator.Validate(new TodoItem{ Label = "", Id = 3});
        
        Assert.False(result.IsValid);
        Assert.InRange(result.Errors.Count, 1, 1);
        Assert.Equal("each TodoItem must have a label", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public void Validate_Complete_Failure_IdNotFound()
    {
        this._repository.Setup(x => x.Get(It.IsAny<string>()))
            .Returns<TodoItem>(null);
        
        var result = this._validator.Validate(new TodoItem{ Label = "Blub", Id = 3}, options => options.IncludeRuleSets(TodoItemRulesets.COMPLETE));
        
        Assert.False(result.IsValid);
        Assert.InRange(result.Errors.Count, 1, 1);
        Assert.Equal("TodoItem must exist and not be archived", result.Errors[0].ErrorMessage);
    }
    
    [Fact]
    public void Validate_Complete_Success()
    {
        this._repository.Setup(x => x.Get(It.IsAny<int>(), false))
            .Returns(new TodoItem{CompletedAt =  DateTime.Now});

        var result = this._validator.Validate(new TodoItem{ Label = "123", Id = 3, CompletedAt =  null}, options => options.IncludeRuleSets(TodoItemRulesets.COMPLETE));
        
        Assert.True(result.IsValid);
    }
}
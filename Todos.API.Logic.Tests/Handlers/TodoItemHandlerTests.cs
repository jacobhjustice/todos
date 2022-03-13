using System;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Todos.API.Logic.Handlers;
using Todos.DTOs.Requests;
using Todos.Models.Entities;
using Todos.Repositories;
using Todos.Utils.Data;
using Todos.Utils.Query;
using Xunit;

namespace Todos.API.Logic.Tests.Handlers;

public class TodoItemHandlerTests
{
    private readonly Mock<IWriteOnlyRepository<TodoItem>> _todoItemWriteRepository;
    private readonly Mock<IReadOnlyTodoItemRepository> _todoItemReadRepository;
    private readonly Mock<IValidator<TodoItem>> _validator;
    
    public TodoItemHandlerTests()
    {
        this._todoItemWriteRepository = new Mock<IWriteOnlyRepository<TodoItem>>();
        this._todoItemReadRepository = new Mock<IReadOnlyTodoItemRepository>();
        this._validator = new Mock<IValidator<TodoItem>>();
    }

    private TodoItemHandler _handler => new TodoItemHandler(this._todoItemWriteRepository.Object, this._todoItemReadRepository.Object, this._validator.Object);

    [Theory]
    [InlineData(1, true)]
    [InlineData(1, false)]
    [InlineData(2, true)]
    [InlineData(2, false)]
    public void Get_Success(int id, bool includeArchived)
    {
        this._handler.Get(id, includeArchived);
        this._todoItemReadRepository.Verify(x => x.Get(id, includeArchived), Times.Once);
    }
    
    [Fact]
    public void Get_Success_Query()
    {
        this._handler.Get(new QueryOptions());
        this._todoItemReadRepository.Verify(x => x.GetAll(It.IsAny<QueryOptions>()), Times.Once);
    }
    
    [Fact]
    public void Create_Failure_Null()
    {
        Assert.Throws<ArgumentNullException>(() => _handler.Create(null));
        
        this._todoItemWriteRepository.Verify(x => x.Add(It.IsAny<TodoItem>()), Times.Never());
        this._todoItemWriteRepository.Verify(x => x.Commit(), Times.Never());
        this._todoItemWriteRepository.Verify(x => x.BeginDatabaseTransaction(), Times.Never());
        this._todoItemWriteRepository.Verify(x => x.CommitDatabaseTransaction(It.IsAny<RepositoryTransaction>()), Times.Never());
    }
    
    [Fact]
    public void Create_Failure_FailedValidation()
    {
        this._validator.Setup(x => x.Validate(It.IsAny<ValidationContext<TodoItem>>()))
            .Returns(new ValidationResult {Errors = {new ValidationFailure("", "")}});
        
        Assert.Throws<Exception>(() => _handler.Create(new TodoItemRequest{Label = "derp"}));
        
        this._todoItemWriteRepository.Verify(x => x.Add(It.IsAny<TodoItem>()), Times.Never());
        this._todoItemWriteRepository.Verify(x => x.Commit(), Times.Never());
        this._todoItemWriteRepository.Verify(x => x.BeginDatabaseTransaction(), Times.Once());
        this._todoItemWriteRepository.Verify(x => x.CommitDatabaseTransaction(It.IsAny<RepositoryTransaction>()), Times.Never());
    }
    
    [Fact]
    public void Create_Success()
    {
        this._validator.Setup(x => x.Validate(It.IsAny<ValidationContext<TodoItem>>()))
            .Returns(new ValidationResult {});

        this._todoItemWriteRepository.Setup(x => x.Add(It.IsAny<TodoItem>()))
            .Returns<TodoItem>(x => x);
        
        var result = _handler.Create(new TodoItemRequest { Label = "derp"});
        Assert.NotNull(result);
        Assert.Equal("derp", result.Label);
        Assert.NotNull(result.CreatedAt);
        Assert.Null(result.ArchivedAt);
        
        this._todoItemWriteRepository.Verify(x => x.Add(It.IsAny<TodoItem>()), Times.Once());
        this._todoItemWriteRepository.Verify(x => x.Commit(), Times.Once());
        this._todoItemWriteRepository.Verify(x => x.BeginDatabaseTransaction(), Times.Once());
        this._todoItemWriteRepository.Verify(x => x.CommitDatabaseTransaction(It.IsAny<RepositoryTransaction>()), Times.Once());
    }
    
    [Fact]
    public void Update_Label_Failure_Null()
    {
        TodoItemRequest req = null;
        Assert.Throws<ArgumentNullException>(() => _handler.Update(req, 1));
        
        this._todoItemReadRepository.Verify(x => x.Get(1, false), Times.Never());
        this._validator.Verify(x => x.Validate(It.IsAny<ValidationContext<TodoItem>>()), Times.Never);
        this._todoItemWriteRepository.Verify(x => x.Update(It.IsAny<TodoItem>()), Times.Never());
        this._todoItemWriteRepository.Verify(x => x.Commit(), Times.Never());
        this._todoItemWriteRepository.Verify(x => x.BeginDatabaseTransaction(), Times.Never());
        this._todoItemWriteRepository.Verify(x => x.CommitDatabaseTransaction(It.IsAny<RepositoryTransaction>()), Times.Never());
    }
    
    [Fact]
    public void Update_Label_Failure_NotFound()
    {
        Assert.Throws<Exception>(() => _handler.Update(new TodoItemRequest{Label = "derp"}, 1));
        
        
        this._todoItemReadRepository.Verify(x => x.Get(1, false), Times.Once());
        this._validator.Verify(x => x.Validate(It.IsAny<ValidationContext<TodoItem>>()), Times.Never);
        this._todoItemWriteRepository.Verify(x => x.Update(It.IsAny<TodoItem>()), Times.Never());
        this._todoItemWriteRepository.Verify(x => x.Commit(), Times.Never());
        this._todoItemWriteRepository.Verify(x => x.BeginDatabaseTransaction(), Times.Once());
        this._todoItemWriteRepository.Verify(x => x.CommitDatabaseTransaction(It.IsAny<RepositoryTransaction>()), Times.Never());
    }
    
    [Fact]
    public void Update_Label_Failure_FailedValidation()
    {
        this._todoItemReadRepository.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns(new TodoItem());
        this._validator.Setup(x => x.Validate(It.IsAny<ValidationContext<TodoItem>>()))
            .Returns(new ValidationResult {Errors = {new ValidationFailure("", "")}});
        
        Assert.Throws<Exception>(() => _handler.Update(new TodoItemRequest{Label = "derp"}, 1));
        
        this._todoItemReadRepository.Verify(x => x.Get(1, false), Times.Once());
        this._validator.Verify(x => x.Validate(It.IsAny<ValidationContext<TodoItem>>()), Times.AtLeastOnce);
        this._todoItemWriteRepository.Verify(x => x.Update(It.IsAny<TodoItem>()), Times.Never());
        this._todoItemWriteRepository.Verify(x => x.Commit(), Times.Never());
        this._todoItemWriteRepository.Verify(x => x.BeginDatabaseTransaction(), Times.Once());
        this._todoItemWriteRepository.Verify(x => x.CommitDatabaseTransaction(It.IsAny<RepositoryTransaction>()), Times.Never());
    }
    
    [Fact]
    public void Update_Label_Success()
    {
        this._validator.Setup(x => x.Validate(It.IsAny<ValidationContext<TodoItem>>()))
            .Returns(new ValidationResult {});

        this._todoItemWriteRepository.Setup(x => x.Update(It.IsAny<TodoItem>()))
            .Returns<TodoItem>(x => x);
        
        this._todoItemReadRepository.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<bool>()))
            .Returns(new TodoItem());
        
        var result = _handler.Update(new TodoItemRequest { Label = "derp"}, 1);
        Assert.NotNull(result);
        Assert.Equal("derp", result.Label);
        Assert.Null(result.ArchivedAt);
        
        this._todoItemReadRepository.Verify(x => x.Get(1, false), Times.Once());
        this._validator.Verify(x => x.Validate(It.IsAny<ValidationContext<TodoItem>>()), Times.AtLeastOnce);
        this._todoItemWriteRepository.Verify(x => x.Update(It.IsAny<TodoItem>()), Times.Once());
        this._todoItemWriteRepository.Verify(x => x.Commit(), Times.Once());
        this._todoItemWriteRepository.Verify(x => x.BeginDatabaseTransaction(), Times.Once());
        this._todoItemWriteRepository.Verify(x => x.CommitDatabaseTransaction(It.IsAny<RepositoryTransaction>()), Times.Once());
    }
    
    [Fact]
    public void Update_Complete_Failure_Null()
    {
        CompleteTodoItemRequest req = null;
        Assert.Throws<ArgumentNullException>(() => _handler.Update(req, 1));
        
        this._todoItemReadRepository.Verify(x => x.Get(1, false), Times.Never());
        this._validator.Verify(x => x.Validate(It.IsAny<ValidationContext<TodoItem>>()), Times.Never);
        this._todoItemWriteRepository.Verify(x => x.Update(It.IsAny<TodoItem>()), Times.Never());
        this._todoItemWriteRepository.Verify(x => x.Commit(), Times.Never());
        this._todoItemWriteRepository.Verify(x => x.BeginDatabaseTransaction(), Times.Never());
        this._todoItemWriteRepository.Verify(x => x.CommitDatabaseTransaction(It.IsAny<RepositoryTransaction>()), Times.Never());
    }
    
    [Fact]
    public void Update_Complete_Failure_NotFound()
    {
        Assert.Throws<Exception>(() => _handler.Update(new CompleteTodoItemRequest{Completed = true}, 1));

        this._todoItemReadRepository.Verify(x => x.Get(1, false), Times.Once());
        this._validator.Verify(x => x.Validate(It.IsAny<ValidationContext<TodoItem>>()), Times.Never);
        this._todoItemWriteRepository.Verify(x => x.Update(It.IsAny<TodoItem>()), Times.Never());
        this._todoItemWriteRepository.Verify(x => x.Commit(), Times.Never());
        this._todoItemWriteRepository.Verify(x => x.BeginDatabaseTransaction(), Times.Once());
        this._todoItemWriteRepository.Verify(x => x.CommitDatabaseTransaction(It.IsAny<RepositoryTransaction>()), Times.Never());
    }
    
    [Fact]
    public void Update_Complete_Failure_FailedValidation()
    {
        this._todoItemReadRepository.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns(new TodoItem());
        this._validator.Setup(x => x.Validate(It.IsAny<ValidationContext<TodoItem>>()))
            .Returns(new ValidationResult {Errors = {new ValidationFailure("", "")}});
        
        Assert.Throws<Exception>(() => _handler.Update(new CompleteTodoItemRequest{Completed = true}, 1));
        
        this._todoItemReadRepository.Verify(x => x.Get(1, false), Times.Once());
        this._validator.Verify(x => x.Validate(It.IsAny<ValidationContext<TodoItem>>()), Times.AtLeastOnce);
        this._todoItemWriteRepository.Verify(x => x.Update(It.IsAny<TodoItem>()), Times.Once());
        this._todoItemWriteRepository.Verify(x => x.Commit(), Times.Never());
        this._todoItemWriteRepository.Verify(x => x.BeginDatabaseTransaction(), Times.Once());
        this._todoItemWriteRepository.Verify(x => x.CommitDatabaseTransaction(It.IsAny<RepositoryTransaction>()), Times.Never());
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Update_Complete_Success(bool isComplete)
    {
        this._validator.Setup(x => x.Validate(It.IsAny<ValidationContext<TodoItem>>()))
            .Returns(new ValidationResult {});

        this._todoItemWriteRepository.Setup(x => x.Update(It.IsAny<TodoItem>()))
            .Returns<TodoItem>(x => x);
        
        this._todoItemReadRepository.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<bool>()))
            .Returns(new TodoItem());
        
        var result = _handler.Update(new CompleteTodoItemRequest { Completed = isComplete}, 1);
        Assert.NotNull(result);
        if (isComplete)
        {
            Assert.NotNull(result.CompletedAt);
        }
        else
        {
            Assert.Null(result.CompletedAt);
        }
        Assert.Null(result.ArchivedAt);
        
        this._todoItemReadRepository.Verify(x => x.Get(1, false), Times.Once());
        this._validator.Verify(x => x.Validate(It.IsAny<ValidationContext<TodoItem>>()), Times.AtLeastOnce);
        this._todoItemWriteRepository.Verify(x => x.Update(It.IsAny<TodoItem>()), Times.Once());
        this._todoItemWriteRepository.Verify(x => x.Commit(), Times.Once());
        this._todoItemWriteRepository.Verify(x => x.BeginDatabaseTransaction(), Times.Once());
        this._todoItemWriteRepository.Verify(x => x.CommitDatabaseTransaction(It.IsAny<RepositoryTransaction>()), Times.Once());
    }
    
    [Fact]
    public void Archive_Failure_NotFound()
    {
        Assert.Throws<Exception>(() => _handler.Archive(1));
        
        this._todoItemReadRepository.Verify(x => x.Get(1, true), Times.Once());
        this._validator.Verify(x => x.Validate(It.IsAny<ValidationContext<TodoItem>>()), Times.Never);
        this._todoItemWriteRepository.Verify(x => x.Archive(It.IsAny<int>()), Times.Never());
        this._todoItemWriteRepository.Verify(x => x.Commit(), Times.Never());
        this._todoItemWriteRepository.Verify(x => x.BeginDatabaseTransaction(), Times.Once());
        this._todoItemWriteRepository.Verify(x => x.CommitDatabaseTransaction(It.IsAny<RepositoryTransaction>()), Times.Never());
    }
    
    [Fact]
    public void Archive_Failure_FailedValidation()
    {
        this._todoItemReadRepository.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns(new TodoItem());
        this._validator.Setup(x => x.Validate(It.IsAny<ValidationContext<TodoItem>>()))
            .Returns(new ValidationResult {Errors = {new ValidationFailure("", "")}});
        
        Assert.Throws<Exception>(() => _handler.Archive(1));
        
        this._todoItemReadRepository.Verify(x => x.Get(1, true), Times.Once());
        this._validator.Verify(x => x.Validate(It.IsAny<ValidationContext<TodoItem>>()), Times.AtLeastOnce);
        this._todoItemWriteRepository.Verify(x => x.Archive(It.IsAny<int>()), Times.Never());
        this._todoItemWriteRepository.Verify(x => x.Commit(), Times.Never());
        this._todoItemWriteRepository.Verify(x => x.BeginDatabaseTransaction(), Times.Once());
        this._todoItemWriteRepository.Verify(x => x.CommitDatabaseTransaction(It.IsAny<RepositoryTransaction>()), Times.Never());
    }
    
    [Fact]
    public void Archive_Success()
    {
        this._todoItemReadRepository.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<bool>())).Returns(new TodoItem());
        
        this._validator.Setup(x => x.Validate(It.IsAny<ValidationContext<TodoItem>>()))
            .Returns(new ValidationResult {});
    
        this._todoItemWriteRepository.Setup(x => x.Archive(It.IsAny<int>()))
            .Returns(new TodoItem{ArchivedAt = new DateTime()});
        
        var result = _handler.Archive(1);
        Assert.NotNull(result);
        Assert.NotNull(result.ArchivedAt);
        
        this._todoItemReadRepository.Verify(x => x.Get(1, true), Times.Once());
        this._validator.Verify(x => x.Validate(It.IsAny<ValidationContext<TodoItem>>()), Times.AtLeastOnce);
        this._todoItemWriteRepository.Verify(x => x.Archive(1), Times.Once());
        this._todoItemWriteRepository.Verify(x => x.Commit(), Times.Once());
        this._todoItemWriteRepository.Verify(x => x.BeginDatabaseTransaction(), Times.Once());
        this._todoItemWriteRepository.Verify(x => x.CommitDatabaseTransaction(It.IsAny<RepositoryTransaction>()), Times.Once());
    }
}
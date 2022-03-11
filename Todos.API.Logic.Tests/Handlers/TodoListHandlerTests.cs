using System;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Todos.API.Logic.Handlers;
using Todos.DTOs.Requests;
using Todos.Models.Entities;
using Todos.Repositories;
using Todos.Utils.Data;
using Xunit;

namespace Todos.API.Logic.Tests.Handlers;

public class TodoListHandlerTests
{
    private readonly Mock<IWriteOnlyRepository<TodoList>> _todoListWriteRepository;
    private readonly Mock<IReadOnlyTodoListRepository> _todoListReadRepository;
    private readonly Mock<IValidator<TodoList>> _validator;
    
    public TodoListHandlerTests()
    {
        this._todoListWriteRepository = new Mock<IWriteOnlyRepository<TodoList>>();
        this._todoListReadRepository = new Mock<IReadOnlyTodoListRepository>();
        this._validator = new Mock<IValidator<TodoList>>();
    }

    private TodoListHandler _handler => new TodoListHandler(this._todoListWriteRepository.Object, this._todoListReadRepository.Object, this._validator.Object);

    [Fact]
    public void Create_Failure_Null()
    {
        Assert.Throws<ArgumentNullException>(() => _handler.Create(null));
        
        this._todoListWriteRepository.Verify(x => x.Add(It.IsAny<TodoList>()), Times.Never());
        this._todoListWriteRepository.Verify(x => x.Commit(), Times.Never());
        this._todoListWriteRepository.Verify(x => x.BeginDatabaseTransaction(), Times.Never());
        this._todoListWriteRepository.Verify(x => x.CommitDatabaseTransaction(It.IsAny<RepositoryTransaction>()), Times.Never());
    }
    
    [Fact]
    public void Create_Failure_FailedValidation()
    {
        this._validator.Setup(x => x.Validate(It.IsAny<ValidationContext<TodoList>>()))
            .Returns(new ValidationResult {Errors = {new ValidationFailure("", "")}});
        
        Assert.Throws<Exception>(() => _handler.Create(new TodoListRequest{Label = "derp"}));
        
        this._todoListWriteRepository.Verify(x => x.Add(It.IsAny<TodoList>()), Times.Never());
        this._todoListWriteRepository.Verify(x => x.Commit(), Times.Never());
        this._todoListWriteRepository.Verify(x => x.BeginDatabaseTransaction(), Times.Once());
        this._todoListWriteRepository.Verify(x => x.CommitDatabaseTransaction(It.IsAny<RepositoryTransaction>()), Times.Never());
    }
    
    [Fact]
    public void Create_Success()
    {
        this._validator.Setup(x => x.Validate(It.IsAny<ValidationContext<TodoList>>()))
            .Returns(new ValidationResult {});

        this._todoListWriteRepository.Setup(x => x.Add(It.IsAny<TodoList>()))
            .Returns<TodoList>(x => x);
        
        var result = _handler.Create(new TodoListRequest { Label = "derp"});
        Assert.NotNull(result);
        Assert.Equal("derp", result.Label);
        Assert.NotNull(result.CreatedAt);
        Assert.Null(result.ArchivedAt);
        
        this._todoListWriteRepository.Verify(x => x.Add(It.IsAny<TodoList>()), Times.Once());
        this._todoListWriteRepository.Verify(x => x.Commit(), Times.Once());
        this._todoListWriteRepository.Verify(x => x.BeginDatabaseTransaction(), Times.Once());
        this._todoListWriteRepository.Verify(x => x.CommitDatabaseTransaction(It.IsAny<RepositoryTransaction>()), Times.Once());
    }
    
    [Fact]
    public void Update_Failure_Null()
    {
        Assert.Throws<ArgumentNullException>(() => _handler.Update(null, 1));
        
        this._todoListWriteRepository.Verify(x => x.Update(It.IsAny<TodoList>()), Times.Never());
        this._todoListWriteRepository.Verify(x => x.Commit(), Times.Never());
        this._todoListWriteRepository.Verify(x => x.BeginDatabaseTransaction(), Times.Never());
        this._todoListWriteRepository.Verify(x => x.CommitDatabaseTransaction(It.IsAny<RepositoryTransaction>()), Times.Never());
    }
    
    [Fact]
    public void Create_Failure_FailedValidation()
    {
        this._validator.Setup(x => x.Validate(It.IsAny<ValidationContext<TodoList>>()))
            .Returns(new ValidationResult {Errors = {new ValidationFailure("", "")}});
        
        Assert.Throws<Exception>(() => _handler.Create(new TodoListRequest{Label = "derp"}));
        
        this._todoListWriteRepository.Verify(x => x.Add(It.IsAny<TodoList>()), Times.Never());
        this._todoListWriteRepository.Verify(x => x.Commit(), Times.Never());
        this._todoListWriteRepository.Verify(x => x.BeginDatabaseTransaction(), Times.Once());
        this._todoListWriteRepository.Verify(x => x.CommitDatabaseTransaction(It.IsAny<RepositoryTransaction>()), Times.Never());
    }
    
    [Fact]
    public void Create_Success()
    {
        this._validator.Setup(x => x.Validate(It.IsAny<ValidationContext<TodoList>>()))
            .Returns(new ValidationResult {});

        this._todoListWriteRepository.Setup(x => x.Add(It.IsAny<TodoList>()))
            .Returns<TodoList>(x => x);
        
        var result = _handler.Create(new TodoListRequest { Label = "derp"});
        Assert.NotNull(result);
        Assert.Equal("derp", result.Label);
        Assert.NotNull(result.CreatedAt);
        Assert.Null(result.ArchivedAt);
        
        this._todoListWriteRepository.Verify(x => x.Add(It.IsAny<TodoList>()), Times.Once());
        this._todoListWriteRepository.Verify(x => x.Commit(), Times.Once());
        this._todoListWriteRepository.Verify(x => x.BeginDatabaseTransaction(), Times.Once());
        this._todoListWriteRepository.Verify(x => x.CommitDatabaseTransaction(It.IsAny<RepositoryTransaction>()), Times.Once());
    }
}
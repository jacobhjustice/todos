using FluentValidation;
using todos.common.Logic;
using Todos.DTOs.Requests;
using Todos.Models.Entities;
using Todos.Repositories;
using Todos.Utils.Data;
using Todos.Utils.Query;
using Todos.Utils.Validation;
using Todos.Validations;

namespace Todos.API.Logic.Handlers;

public class TodoItemHandler : ITodoItemHandler
{
    private readonly IWriteOnlyRepository<TodoItem> _todoItemWriteRepository;
    private readonly IReadOnlyTodoItemRepository _todoItemReadRepository;
    private readonly IValidator<TodoItem> _validator;

    public TodoItemHandler(IWriteOnlyRepository<TodoItem> todoItemWriteRepository, IReadOnlyTodoItemRepository todoodoItemReadRepository, IValidator<TodoItem> validator)
    {
        this._todoItemWriteRepository = todoItemWriteRepository;
        this._todoItemReadRepository = todoodoItemReadRepository;
        this._validator = validator;
    }

    public TodoItem Create(TodoItemRequest req)
    {
        if (req == null)
        {
            throw new ArgumentNullException();
        }

        var transaction = this._todoItemWriteRepository.BeginDatabaseTransaction();

        var item = new TodoItem
        {
            Label = req.Label,
            TodoListId = req.ListId
        };

        var results = this._validator.Validate(item, options => options.IncludeRuleSets(Rulesets.CREATE));
        if (!results.IsValid)
        {
            throw new Exception(String.Join(", ", results.Errors.Select(x => x.ErrorMessage)));
        }
        
        item = this._todoItemWriteRepository.Add(item);
        this._todoItemWriteRepository.Commit();

        this._todoItemWriteRepository.CommitDatabaseTransaction(transaction);
        return item;
    }

    public TodoItem Update(CompleteTodoItemRequest req, int id)
    {
        if (req == null)
        {
            throw new ArgumentNullException();
        }
        
        var transaction = this._todoItemWriteRepository.BeginDatabaseTransaction();

        var item = this._todoItemReadRepository.Get(id, false);
        if (item == null)
        {
            throw new Exception($"TodoItem with id {id} not found");
        }

        item.CompletedAt = req.Completed ? DateTime.Now : null;
        
        item = this._todoItemWriteRepository.Update(item);

        var results = this._validator.Validate(item, options => options.IncludeRuleSets(TodoItemRulesets.COMPLETE));
        if (!results.IsValid)
        {
            throw new Exception(String.Join(", ", results.Errors.Select(x => x.ErrorMessage)));
        }
        
        this._todoItemWriteRepository.Commit();

        this._todoItemWriteRepository.CommitDatabaseTransaction(transaction);

        return item;
    }

    public TodoItem Update(TodoItemRequest req, int id)
    {
        if (req == null)
        {
            throw new ArgumentNullException();
        }

        var transaction = this._todoItemWriteRepository.BeginDatabaseTransaction();

        var item = this._todoItemReadRepository.Get(id, false);
        if (item == null)
        {
            throw new Exception($"TodoItem with id {id} not found");
        }
        
        item.Label = req.Label;
        
        var results = this._validator.Validate(item, options => options.IncludeRuleSets(Rulesets.UPDATE));
        if (!results.IsValid)
        {
            throw new Exception(String.Join(", ", results.Errors.Select(x => x.ErrorMessage)));
        }
        
        item = this._todoItemWriteRepository.Update(item);
        this._todoItemWriteRepository.Commit();

        this._todoItemWriteRepository.CommitDatabaseTransaction(transaction);

        return item;
    }
    
    public TodoItem Archive(int id)
    {
        var transaction = this._todoItemWriteRepository.BeginDatabaseTransaction();

        var item = this._todoItemReadRepository.Get(id, true);
        if (item == null)
        {
            throw new Exception($"TodoItem with id {id} not found");
        }
        
        var results = this._validator.Validate(item, options => options.IncludeRuleSets(Rulesets.ARCHIVE));
        if (!results.IsValid)
        {
            throw new Exception(String.Join(", ", results.Errors.Select(x => x.ErrorMessage)));
        }
        
        item = this._todoItemWriteRepository.Archive(id);
        this._todoItemWriteRepository.Commit();

        this._todoItemWriteRepository.CommitDatabaseTransaction(transaction);

        return item;
    }
    
    public IList<TodoItem> Get(QueryOptions options)
    {
        var items = this._todoItemReadRepository.GetAll(options);
        return items.ToList();
    }
    
    public IList<TodoItem> Get(TodoItemQueryOptions options)
    {
        var items = this._todoItemReadRepository.GetAll(options);
        return items.ToList();
    }
    
    public TodoItem? Get(int id, bool includeArchived)
    {
        var item = this._todoItemReadRepository.Get(id, includeArchived);
        return item;
    }
}
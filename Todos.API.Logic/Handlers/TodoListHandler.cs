using FluentValidation;
using todos.common.Logic;
using Todos.DTOs.Requests;
using Todos.Models.Entities;
using Todos.Repositories;
using Todos.Utils.Data;
using Todos.Utils.Query;
using Todos.Utils.Validation;

namespace Todos.API.Logic.Handlers;

public class TodoListHandler : IHandler<TodoList, TodoListRequest>
{
    private readonly IWriteOnlyRepository<TodoList> _todoListWriteRepository;
    private readonly IReadOnlyTodoListRepository _todoListReadRepository;
    private readonly IValidator<TodoList> _validator;

    public TodoListHandler(IWriteOnlyRepository<TodoList> todoListWriteRepository, IReadOnlyTodoListRepository todoListReadRepository, IValidator<TodoList> validator)
    {
        this._todoListReadRepository = todoListReadRepository;
        this._todoListWriteRepository = todoListWriteRepository;
        this._validator = validator;
    }

    public TodoList Create(TodoListRequest req)
    {
        if (req == null)
        {
            throw new ArgumentNullException();
        }

        var transaction = this._todoListWriteRepository.BeginDatabaseTransaction();

        var list = new TodoList
        {
            Label = req.Label
        };

        var results = this._validator.Validate(list, options => options.IncludeRuleSets(Rulesets.CREATE));
        if (!results.IsValid)
        {
            throw new Exception(String.Join(", ", results.Errors.Select(x => x.ErrorMessage)));
        }
        
        list = this._todoListWriteRepository.Add(list);
        this._todoListWriteRepository.Commit();

        this._todoListWriteRepository.CommitDatabaseTransaction(transaction);
        return list;
    }
    
    public TodoList Update(TodoListRequest req, int id)
    {
        if (req == null)
        {
            throw new ArgumentNullException();
        }

        var transaction = this._todoListWriteRepository.BeginDatabaseTransaction();

        var list = this._todoListReadRepository.Get(id, false);
        if (list == null)
        {
            throw new Exception($"TodoList with id {id} not found");
        }
        
        list.Label = req.Label;
        
        var results = this._validator.Validate(list, options => options.IncludeRuleSets(Rulesets.UPDATE));
        if (!results.IsValid)
        {
            throw new Exception(String.Join(", ", results.Errors.Select(x => x.ErrorMessage)));
        }
        
        list = this._todoListWriteRepository.Update(list);
        this._todoListWriteRepository.Commit();

        this._todoListWriteRepository.CommitDatabaseTransaction(transaction);

        return list;
    }
    
    public TodoList Archive(int id)
    {
        var transaction = this._todoListWriteRepository.BeginDatabaseTransaction();

        var list = this._todoListReadRepository.Get(id, true);
        if (list == null)
        {
            throw new Exception($"TodoList with id {id} not found");
        }
        
        var results = this._validator.Validate(list, options => options.IncludeRuleSets(Rulesets.ARCHIVE));
        if (!results.IsValid)
        {
            throw new Exception(String.Join(", ", results.Errors.Select(x => x.ErrorMessage)));
        }
        
        list = this._todoListWriteRepository.Archive(id);
        this._todoListWriteRepository.Commit();

        this._todoListWriteRepository.CommitDatabaseTransaction(transaction);

        return list;
    }
    
    public IList<TodoList> Get(QueryOptions options)
    {
        var lists = this._todoListReadRepository.GetAll(options);
        return lists.ToList();
    }
    
    public TodoList? Get(int id, bool includeArchived)
    {
        var list = this._todoListReadRepository.Get(id, includeArchived);
        return list;
    }
}
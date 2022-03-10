using todos.common.Logic;
using Todos.DTOs.Requests;
using Todos.Models.Entities;
using Todos.Utils.Data;
using Todos.Utils.Query;

namespace Todos.API.Logic.Handlers;

public class TodoListHandler : IHandler<TodoList, TodoListRequest>
{
    private readonly IWriteOnlyRepository<TodoList> _todoListWriteRepository;
    private readonly IReadOnlyRepository<TodoList> _todoListReadRepository;
    
    public TodoListHandler(IWriteOnlyRepository<TodoList> todoListWriteRepository, IReadOnlyRepository<TodoList> todoListReadRepository)
    {
        this._todoListReadRepository = todoListReadRepository;
        this._todoListWriteRepository = todoListWriteRepository;
    }

    public TodoList Create(TodoListRequest req)
    {
        if (req == null)
        {
            throw new ArgumentNullException();
        }

        var list = new TodoList
        {
            Label = req.Label
        };
        
        list = this._todoListWriteRepository.Add(list);
        this._todoListWriteRepository.Commit();

        return list;
    }
    
    public TodoList Update(TodoListRequest req, int id)
    {
        if (req == null)
        {
            throw new ArgumentNullException();
        }

        var list = this._todoListReadRepository.Get(id, false);
        if (list == null)
        {
            throw new Exception($"TodoList with id {id} not found");
        }
        
        list.Label = req.Label;
        list = this._todoListWriteRepository.Update(list);
        this._todoListWriteRepository.Commit();

        return list;
    }
    
    public TodoList Archive(int id)
    {
        var list = this._todoListWriteRepository.Archive(id);
        this._todoListWriteRepository.Commit();

        return list;
    }
    
    public IList<TodoList> Get(QueryOptions options)
    {
        var lists = this._todoListReadRepository.GetAll(options);
        return lists.ToList();
    }
    
    public TodoList Get(int id)
    {
        var list = this._todoListReadRepository.Get(id, true);
        return list;
    }
}
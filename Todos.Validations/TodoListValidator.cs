using FluentValidation;
using Todos.Models.Entities;
using Todos.Repositories;
using Todos.Utils.Data;
using Todos.Utils.Validation;

namespace Todos.Validations;

public class TodoListValidator: AbstractValidator<TodoList>
{
    private readonly IReadOnlyTodoListRepository _repository;
    public TodoListValidator(IReadOnlyTodoListRepository repository)
    {
        this._repository = repository;
        
        RuleSet(Rulesets.CREATE, () =>
        {
            RuleFor(x => x.Label)
                .Must(this.NotExist)
                .WithMessage("each TodoList label must be unique");
        });
        
        RuleSet(Rulesets.UPDATE, () =>
        {
            RuleFor(x => x.Id)
                .Must(this.MustExist)
                .WithMessage("id must reference an actual TodoList");
            
            RuleFor(x => x.Label)
                .Must(this.NotExist)
                .WithMessage("each TodoList label must be unique");
        });
        
        RuleSet(Rulesets.ARCHIVE, () =>
        {
            RuleFor(x => x.Id)
                .Must(this.MustExist)
                .WithMessage("id must reference an actual TodoList");
            
            RuleFor(x => x.Id)
                .Must(this.NotArchived)
                .WithMessage("cannot archive a TodoList that is already archived");
        });
    }

    public bool NotExist(string label)
    {
        return this._repository.Get(label) == null;
    }
    
    public bool MustExist(int id)
    {
        return this._repository.Get(id, true) != null;
    }
    
    public bool NotArchived(int id)
    {
        return !this._repository.Get(id, true)?.ArchivedAt.HasValue ?? true;
    }
}
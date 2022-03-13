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

        RuleFor(x => x.Label)
            .NotEmpty()
            .WithMessage("each TodoList must have a label");
        
        RuleSet(Rulesets.CREATE, () =>
        {
            RuleFor(x => x.Label)
                .Must(this.NotExist)
                .WithMessage("each TodoList label must be unique");
        });
        
        RuleSet(Rulesets.UPDATE, () =>
        {
            RuleFor(x => x.Id)
                .Must(this.NotArchived)
                .WithMessage("TodoList must exist and not be archived");
            
            RuleFor(x => x.Label)
                .Must(this.NotExist)
                .WithMessage("each TodoList label must be unique");
        });
        
        RuleSet(Rulesets.ARCHIVE, () =>
        {
            RuleFor(x => x.Id)
                .Must(this.NotArchived)
                .WithMessage("TodoList must exist and not be archived");
        });
    }

    public bool NotExist(string label)
    {
        return this._repository.Get(label) == null;
    }
    
    public bool NotArchived(int id)
    {
        return this._repository.Get(id, false) != null;
    }
}
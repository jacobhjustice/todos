using FluentValidation;
using Todos.DTOs.Requests;
using Todos.Models.Entities;
using Todos.Repositories;
using Todos.Utils.Data;
using Todos.Utils.Query;
using Todos.Utils.Validation;

namespace Todos.Validations;

public class TodoItemValidator: AbstractValidator<TodoItem>
{
    private readonly IReadOnlyTodoItemRepository _repository;
    public TodoItemValidator(IReadOnlyTodoItemRepository repository)
    {
        this._repository = repository;

        RuleFor(x => x.Label)
            .NotEmpty()
            .WithMessage("each TodoItem must have a label");
        
        RuleSet(TodoItemRulesets.COMPLETE, () =>
        {
            RuleFor(x => x.Id)
                .Must(this.NotArchived)
                .WithMessage("TodoItem must exist and not be archived");
        });
        
        RuleSet(Rulesets.CREATE, () =>
        {
            RuleFor(x => x.Label)
                .Must(this.NotExist)
                .WithMessage("each TodoItem label must be unique");
        });
        
        RuleSet(Rulesets.UPDATE, () =>
        {
            RuleFor(x => x.Id)
                .Must(this.NotArchived)
                .WithMessage("TodoItem must exist and not be archived");
            
            RuleFor(x => x.Label)
                .Must(this.NotExist)
                .WithMessage("each TodoItem label must be unique");
        });
        
        RuleSet(Rulesets.ARCHIVE, () =>
        {
            RuleFor(x => x.Id)
                .Must(this.NotArchived)
                .WithMessage("TodoItem must exist and not be archived");
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
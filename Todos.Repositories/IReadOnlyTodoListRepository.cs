using Todos.Models.Entities;
using Todos.Utils.Data;

namespace Todos.Repositories;

public interface IReadOnlyTodoListRepository: IReadOnlyRepository<TodoList>
{
    TodoList? Get(string label);
}
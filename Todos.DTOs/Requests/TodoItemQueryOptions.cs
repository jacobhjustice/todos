using Todos.Utils.Query;

namespace Todos.DTOs.Requests;

public class TodoItemQueryOptions : QueryOptions
{
    public int? TodoListId { get; set; }
    public bool? Completed { get; set; }
}
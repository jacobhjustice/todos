using Todos.Models.Entities;

namespace Todos.DTOs.Responses;

public class TodoListResponse
{
    public int Id { get; set; }
    public string Label { get; set; }
    public bool IsArchived { get; set; }

    public TodoListResponse(TodoList list)
    {
        this.Id = list.Id;
        this.Label = list.Label;
        this.IsArchived = list.ArchivedAt.HasValue;
    }
}
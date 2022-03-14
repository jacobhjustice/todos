using Todos.Models.Entities;

namespace Todos.DTOs.Responses;

public class TodoItemResponse
{
    public int Id { get; set; }
    public int TodoListId { get; set; }
    public string Label { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsArchived { get; set; }
    
    public TodoItemResponse(TodoItem item)
    {
        this.Id = item.Id;
        this.Label = item.Label;
        this.IsArchived = item.ArchivedAt.HasValue;
        this.TodoListId = item.TodoListId;
        this.IsCompleted = item.CompletedAt.HasValue;
    }
}
namespace Todos.DTOs.Responses;

public class TodoItemResponse
{
    public int Id { get; set; }
    public int TodoListId { get; set; }
    public string Label { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsArchived { get; set; }
}
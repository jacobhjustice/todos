namespace Todos.DTOs.Requests;

public class PatchCompleteTodoItemRequest
{
    public DateTime? CompletedAt { get; set; }
}
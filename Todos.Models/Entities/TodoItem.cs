using Todos.Utils.Data;

namespace Todos.Models.Entities;

public class TodoItem : IDataRecord
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public int TodoListId { get; set; }
    public string Label { get; set; }
    public DateTime? CompletedAt { get; set; }
    public virtual TodoList TodoList { get; set; }
}
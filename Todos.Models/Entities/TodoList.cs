using Todos.Utils.Data;

namespace Todos.Models.Entities;

public class TodoList: IDataRecord
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public virtual IList<TodoItem> TodoItems { get; set; }
}
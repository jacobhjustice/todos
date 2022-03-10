namespace Todos.Utils.Data;

public interface IDataRecord
{
    int Id { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime? ArchivedAt { get; set; }
}
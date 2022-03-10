namespace Todos.Utils.Query;

public class QueryOptions
{
    public int? Limit { get; set; }
    public int? Offset { get; set; }
    public string? Order { get; set; }
    public bool IsDescending { get; set; }
    public bool IncludeArchived { get; set; }
}
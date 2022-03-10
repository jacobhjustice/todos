namespace todos.common.Data;

public interface IQueryOptions
{
    int? Limit { get; set; }
    int? Offset { get; set; }
    string? Order { get; set; }
    bool IsDescending { get; set; }
    bool IncludeArchived { get; set; }
}
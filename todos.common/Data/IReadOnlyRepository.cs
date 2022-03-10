namespace todos.common.Data;

public interface IReadOnlyRepository<T> where T : class, IDataRecord
{
    T? Get(int id, bool includeArchived);
    IQueryable<T> GetAll(IQueryOptions? options);
}
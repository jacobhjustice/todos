using Todos.Utils.Query;

namespace Todos.Utils.Data;
public interface IReadOnlyRepository<T> where T : class, IDataRecord
{
    T? Get(int id, bool includeArchived);
    IQueryable<T> GetAll(QueryOptions? options);
}
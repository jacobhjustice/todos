using Todos.Utils.Data;
using Todos.Utils.Query;

namespace todos.common.Logic;

public interface IHandler<T, R> where T : class, IDataRecord where R : class
{
    T Create(R req);
    T Update(R req, int id);
    T Archive(int id);
    IList<T> Get(IQueryOptions options);
    T Get(int id);
}
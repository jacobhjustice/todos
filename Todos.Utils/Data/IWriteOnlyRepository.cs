namespace Todos.Utils.Data;
public interface IWriteOnlyRepository<T> where T : class, IDataRecord
{
        T Add(T recordToAdd);
        T Update(T recordToUpdate);
        T Archive(int id);
        RepositoryTransaction BeginDatabaseTransaction();
        bool CommitDatabaseTransaction(RepositoryTransaction transaction);
        int Commit();
}
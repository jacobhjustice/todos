namespace Todos.Utils.Data;
public interface IWriteOnlyRepository<T> where T : class, IDataRecord
{
        T Add(T recordToAdd);
        T Update(T recordToUpdate);
        T Archive(int id);
        RepositoryTransaction BeginDatabaseTransaction();
        void CommitDatabaseTransaction(RepositoryTransaction transaction);
        int Commit();
}
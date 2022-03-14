using Microsoft.EntityFrameworkCore.Storage;

namespace Todos.Utils.Data;

public class RepositoryTransaction: IDisposable
{
    private readonly IDbContextTransaction _transaction;
    private readonly bool _isTopLevel;

    public int Id => this._transaction.GetHashCode();
    
    public RepositoryTransaction(IDbContextTransaction transaction, bool isTopLevel)
    {
        this._transaction = transaction;
        this._isTopLevel = isTopLevel;
    }

    public bool Commit()
    {
        if (this._isTopLevel)
        {
            this._transaction.Commit();
            return true;
        }

        return false;
    }

    public void Dispose()
    {
        if (this._isTopLevel)
        {
            this._transaction.Dispose();
        }
    }
}

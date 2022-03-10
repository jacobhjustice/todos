using Microsoft.EntityFrameworkCore.Storage;

namespace todos.common.Data;

public class RepositoryTransaction: IDisposable
{
    private readonly IDbContextTransaction _transaction;
    private readonly bool _isTopLevel;
    
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

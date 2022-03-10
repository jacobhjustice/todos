using Microsoft.EntityFrameworkCore;
using Todos.Utils.Query;

namespace Todos.Utils.Data;
public abstract class ReadWriteRepository<T>: IReadOnlyRepository<T>, IWriteOnlyRepository<T> where T: class, IDataRecord
{
    private readonly DbContext _context;
    private DbSet<T> _data => this._context.Set<T>();

    public ReadWriteRepository(DbContext context)
    {
        this._context = context;
    }

    public T Add(T recordToAdd)
    {
        recordToAdd.CreatedAt = DateTime.Now;
        this._data.Add(recordToAdd);
        return recordToAdd;
    }

    public T Update(T recordToUpdate)
    {
        this._data.Update(recordToUpdate);
        return recordToUpdate;
    }
    
    public T Archive(int id)
    {
        var entity = this.Get(id, false);
        if (entity == null)
        {
            throw new Exception($"un-archived entity of type {typeof(T)} with id of {id} not found during archive");
        }
        
        entity.ArchivedAt = DateTime.Now;
        this._data.Update(entity);
        return entity;
    }
    
    public T? Get(int id, bool includeArchived)
    {
        return this._data.FirstOrDefault(x => x.Id == id && (x.ArchivedAt == null || includeArchived));
    }
    
    // Note: If you want to provide entity-specific filtering,
    // override this call in child with some child of IQueryOptions.
    // If you do this, be sure to call the base function to leverage the existing query options
    // and do not materialize the queryable until you are ready to apply all filters
    public virtual IQueryable<T> GetAll(IQueryOptions? options)
    {
        var query = this._data.AsQueryable();
        if (options != null)
        {
            if (options.Limit.HasValue)
            {
                if (!options.Offset.HasValue)
                {
                    options.Offset = 1;
                }

                query = query
                    .Skip(options.Offset.Value * options.Limit.Value)
                    .Take(options.Limit.Value)
                    .AsQueryable();
            }

            if (!string.IsNullOrEmpty(options.Order))
            {
                if (typeof(T).GetProperty(options.Order) == null)
                {
                    throw new Exception($"entity of type {typeof(T)} does not have property {options.Order}");
                }

                query = query
                    .OrderBy(s => s.GetType().GetProperty(options.Order).GetValue(s))
                    .AsQueryable();
            }

            query = query
                .Where(x => x.ArchivedAt == null || options.IncludeArchived)
                .AsQueryable();
        }

        return query;
    }
    
    public virtual RepositoryTransaction BeginDatabaseTransaction()
    {
        var t = this._context.Database.CurrentTransaction;
        if (t == null)
        {
            return new RepositoryTransaction(this._context.Database.BeginTransaction(), true);
        }

        return new RepositoryTransaction(t, false);
    } 

    public virtual void CommitDatabaseTransaction(RepositoryTransaction transaction) => transaction.Commit();
    
    public int Commit()
    {
        return this._context.SaveChanges();
    }

}
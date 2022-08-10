using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ManageMoneyServer.Repositories
{
    public enum IncludeType
    {
        Reference,
        Collection
    }
    public interface IRepository<TEntity> : IAsyncDisposable where TEntity : class
    {
        Task<TEntity> CreateAsync(TEntity item);
        Task<IEnumerable<TEntity>> CreateAsync(params TEntity[] items);
        Task<TEntity> FindByIdAsync(int id);
        Task<TEntity> FindByIdAsync(int id, params Tuple<IncludeType, string>[] includes);
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);
        Task<IQueryable<TEntity>> GetAsync();
        Task<List<TEntity>> GetListAsync();
        Task<IQueryable<TEntity>> GetAsync(params Expression<Func<TEntity, object>>[] includes);
        Task<List<TEntity>> GetListAsync(params Expression<Func<TEntity, object>>[] includes);
        Task<IQueryable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate);
        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IQueryable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);
        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);
        Task RemoveAsync(int id);
        Task RemoveAsync(TEntity item);
        Task RemoveAsync(params TEntity[] item);
        Task RemoveAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> UpdateAsync(TEntity item);
        Task<IEnumerable<TEntity>> UpdateAsync(params TEntity[] items);
        Task Attach(params TEntity[] items);
        Task SaveAsync();
    }
}

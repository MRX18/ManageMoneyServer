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
        Task<bool> HasAsync(Expression<Func<TEntity, bool>> predicate);
        Task<bool> HasAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity> CreateAsync(TEntity item);
        Task<TEntity> CreateDetachedAsync(TEntity item);
        Task<IEnumerable<TEntity>> CreateAsync(params TEntity[] items);
        Task<IEnumerable<TEntity>> CreateDetachedAsync(params TEntity[] items);
        Task<TEntity> FindByIdAsync(int id, bool detached = false);
        Task<TEntity> FindByIdAsync(int id, bool detached = false, params Tuple<IncludeType, string>[] includes);
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, bool detached = false);
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, bool detached = false, params Expression<Func<TEntity, object>>[] includes);
        Task<IQueryable<TEntity>> GetAsync(bool detached = false);
        Task<List<TEntity>> GetListAsync(bool detached = false);
        Task<IQueryable<TEntity>> GetAsync(bool detached = false, params Expression<Func<TEntity, object>>[] includes);
        Task<List<TEntity>> GetListAsync(bool detached = false, params Expression<Func<TEntity, object>>[] includes);
        Task<IQueryable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, bool detached = false);
        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, bool detached = false);
        Task<IQueryable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, bool detached = false, params Expression<Func<TEntity, object>>[] includes);
        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, bool detached = false, params Expression<Func<TEntity, object>>[] includes);
        Task RemoveAsync(int id);
        Task RemoveAsync(TEntity item);
        Task RemoveAsync(params TEntity[] item);
        Task RemoveAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> UpdateAsync(TEntity item);
        Task<TEntity> UpdateDetachedAsync(TEntity item);
        Task<IEnumerable<TEntity>> UpdateAsync(params TEntity[] items);
        Task<IEnumerable<TEntity>> UpdateDetachedAsync(params TEntity[] items);
        Task Attach(params TEntity[] items);
        Task SaveAsync();
    }
}

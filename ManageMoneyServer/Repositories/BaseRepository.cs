﻿using ManageMoneyServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ManageMoneyServer.Repositories
{
    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private ApplicationContext DbContext { get; set; }
        protected DbSet<TEntity> Entity { get; set; }
        public BaseRepository(ApplicationContext dbContext)
        {
            DbContext = dbContext;
            Entity = DbContext.Set<TEntity>();
        }

        public async Task<TEntity> CreateAsync(TEntity item)
        {
            EntityEntry entry = await Entity.AddAsync(item);
            entry.State = EntityState.Detached;
            await SaveAsync();
            return item;
        }

        public async Task<IEnumerable<TEntity>> CreateAsync(params TEntity[] items)
        {
            await Entity.AddRangeAsync(items);
            await SaveAsync();

            foreach(TEntity item in items)
            {
                DbContext.Entry(item).State = EntityState.Detached;
            }

            return items;
        }

        public async Task<TEntity> FindByIdAsync(int id)
        {
            TEntity item = await Entity.FindAsync(id);
            DbContext.Entry(item).State = EntityState.Detached;
            return item;
        }

        public async Task<TEntity> FindByIdAsync(int id, params Tuple<IncludeType, string>[] includes)
        {
            TEntity item = await Entity.FindAsync(id);
            EntityEntry<TEntity> entry = DbContext.Entry(item);
            
            foreach(Tuple<IncludeType, string> include in includes)
            {
                switch(include.Item1)
                {
                    case IncludeType.Collection: await entry.Collection(include.Item2).LoadAsync(); break;
                    case IncludeType.Reference: await entry.Reference(include.Item2).LoadAsync(); break;
                }
            }

            entry.State = EntityState.Detached;

            return item;
        }

        public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Entity.AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = Entity.AsNoTracking();
            return includes.Aggregate(query, (current, next) => current.Include(next)).FirstOrDefaultAsync(predicate);
        }

        public Task<IQueryable<TEntity>> GetAsync()
        {
            return Task.FromResult(Entity.AsNoTracking().AsQueryable());
        }

        public async Task<List<TEntity>> GetListAsync() => await (await GetAsync()).ToListAsync();

        public Task<IQueryable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(Entity.AsNoTracking().Where(predicate).AsQueryable());
        }

        public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate) => await (await GetAsync(predicate)).ToListAsync();

        public Task<IQueryable<TEntity>> GetAsync(params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = Entity.AsNoTracking();
            return Task.FromResult(includes.Aggregate(query, (current, next) => current.Include(next)));
        }

        public async Task<List<TEntity>> GetListAsync(params Expression<Func<TEntity, object>>[] includes) => await (await GetAsync(includes)).ToListAsync();

        public Task<IQueryable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = Entity.AsNoTracking();
            return Task.FromResult(includes.Aggregate(query, (current, next) => current.Include(next)).Where(predicate));
        }

        public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
            => await (await GetAsync(predicate, includes)).ToListAsync();

        public async Task RemoveAsync(int id)
        {
            TEntity entity = await FindByIdAsync(id);
            await RemoveAsync(entity);
        }

        public async Task RemoveAsync(TEntity item)
        {
            Entity.Remove(item);
            await SaveAsync();
        }

        public async Task RemoveAsync(params TEntity[] items)
        {
            Entity.RemoveRange(items);
            await SaveAsync();
        }

        public async Task RemoveAsync(Expression<Func<TEntity, bool>> predicate)
        {
            IQueryable<TEntity> entities = await GetAsync(predicate);
            Entity.RemoveRange(entities);
            await SaveAsync();
        }

        public async Task<TEntity> UpdateAsync(TEntity item)
        {
            EntityEntry entry = Entity.Attach(item);
            Entity.Update(item);
            await SaveAsync();
            entry.State = EntityState.Detached;
            return item;
        }

        public async Task<IEnumerable<TEntity>> UpdateAsync(params TEntity[] items)
        {
            Entity.UpdateRange(items);
            await SaveAsync();

            foreach (TEntity item in items)
            {
                DbContext.Entry(item).State = EntityState.Detached;
            }

            return items;
        }

        public Task Attach(params TEntity[] items)
        {
            DbContext.ChangeTracker.Clear();
            Entity.AttachRange(items);
            return Task.CompletedTask;
        }

        public async Task SaveAsync()
        {
            await DbContext.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await SaveAsync();
        }
    }
}
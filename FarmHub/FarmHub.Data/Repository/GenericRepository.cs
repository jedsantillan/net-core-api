using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FarmHub.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FarmHub.Data.Repository
{
    /// <summary>
    /// GenericRepository for basic CRUD functionalities
    /// </summary>
    /// <typeparam name="T">T is a class of type ModelBase</typeparam>
    /// <typeparam name="T1">Pass a database context</typeparam>
    public class GenericRepository<T, T1> : IGenericRepository<T>
        where T : ModelBase
        where T1 : DbContext
    {
        protected readonly T1 _dbContext;

        // https://docs.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.infrastructure.iresettableservice?view=efcore-3.1
        public GenericRepository(IResettableService dbContext)
        {
            _dbContext = dbContext as T1;
        }

        public async Task InsertAsync(T obj)
        {
            obj.CreatedDate = DateTime.UtcNow;
            obj.ModifiedDate = DateTime.UtcNow;
            await _dbContext.AddAsync(obj);
            await _dbContext.SaveChangesAsync();
        }

        public async Task InsertRangeAsync(IEnumerable<T> objs)
        {
            var createdDate = DateTime.UtcNow;
            var modifiedDate = DateTime.UtcNow;
            var modelBases = objs.ToList();
            foreach (var o in modelBases)
            {
                o.CreatedDate = createdDate;
                o.ModifiedDate = modifiedDate;
            }
            await _dbContext.AddRangeAsync(modelBases);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(T obj)
        {
            obj.ModifiedDate = DateTime.UtcNow;
            _dbContext.Update(obj);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(T obj)
        {
            _dbContext.Remove(obj);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteById(int id)
        {
            var obj = await _dbContext.FindAsync<T>(id);
            _dbContext.Remove(obj);
            await _dbContext.SaveChangesAsync();
        }

        public List<T> GetAll()
        {
            return _dbContext.Set<T>().ToList();
        }

        public async Task<List<T>> GetAllListAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        
        public IQueryable<T> GetWhere(Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>().Where(predicate);
        }

        public async Task<int> CountAllAsync()
        {
            return await _dbContext.Set<T>().CountAsync();
        }

        public async Task<int> CountWhereAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().CountAsync(predicate);
        }

        public IQueryable<T> GetAllQueryable()
        {
            return _dbContext.Set<T>();
        }

        public Task<List<T>> FilterAsync(Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>().Where(predicate).ToListAsync();
        }

        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>().FirstOrDefaultAsync(predicate);
        }
        public bool IsIdExists(int id)
        {
            return _dbContext.Set<T>().Any(x => x.Id == id);
        }

    }
}
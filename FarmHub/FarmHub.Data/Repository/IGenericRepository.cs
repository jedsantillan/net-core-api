using FarmHub.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FarmHub.Data.Repository
{
    public interface IGenericRepository<T>
        where T : ModelBase
    {
        Task InsertAsync(T obj);
        Task InsertRangeAsync(IEnumerable<T> obj);
        Task Update(T obj);
        Task Delete(T obj);
        Task DeleteById(int id);
        IQueryable<T> GetAllQueryable();
        Task<List<T>> GetAllListAsync();
        List<T> GetAll();
        Task<T> GetByIdAsync(int id);
        Task<List<T>> FilterAsync(Expression<Func<T, bool>> predicate);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAllAsync();
        Task<int> CountWhereAsync(Expression<Func<T, bool>> predicate);
        bool IsIdExists(int id);
    }
}
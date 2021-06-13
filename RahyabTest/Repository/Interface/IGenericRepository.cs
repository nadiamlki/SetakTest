using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SetakTest.Repository.Interface
{
    public interface IGenericRepository<TId, TEntity>
    {
        Task<List<TEntity>> GetAll();
        Task<TEntity> Add(TEntity obj);       
        void Update(TEntity  obj);
        Task SaveChanges();
        Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>> Where = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderby = null, string includes = "");

    }
}

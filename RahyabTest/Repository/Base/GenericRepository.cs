using Microsoft.EntityFrameworkCore;
using SetakTest.Data;
using SetakTest.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SetakTest.Repository
{
    public class GenericRepository<TId, TEntity> : IGenericRepository<TId, TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _Db;
        private DbSet<TEntity> _dbset;
        public GenericRepository(ApplicationDbContext Db)
        {
            _Db = Db;
            _dbset = _Db.Set<TEntity>();
        }

        public async Task<TEntity> Add(TEntity obj)
        {
            await _Db.Set<TEntity>().AddAsync(obj);
            return obj;
        }

        public async Task<List<TEntity>> GetAll()
        {
            return await _Db.Set<TEntity>().ToListAsync();
        }

        public async Task SaveChanges()
        {
            await _Db.SaveChangesAsync();
        }

        public  void Update(TEntity obj)
        {
            _Db.Set<TEntity>().Update(obj);
        }

        public async Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>> Where = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderby = null, string includes = "")
        {
            IQueryable<TEntity> query = _dbset;
            if (Where != null)
            {
                query = query.Where(Where);
            }
            if (orderby != null)
            {
                query = orderby(query);
            }
            if (includes != string.Empty)
            {
                foreach (string include in includes.Split(','))
                {
                    query = query.Include(includes);
                }
            }
            return await query.ToListAsync();
        }

    }
}

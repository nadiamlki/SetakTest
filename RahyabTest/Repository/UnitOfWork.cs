using SetakTest.Data;
using SetakTest.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SetakTest.Repository
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly ApplicationDbContext dbcontext;
        public IUserRepository Users { get; }
        public IRoleRepository Roles { get; }
        public UnitOfWork(ApplicationDbContext _dbcontext, IUserRepository userRepository, IRoleRepository roleRepository)
        {
            this.dbcontext = _dbcontext;
            this.Users = userRepository;
            this.Roles = roleRepository;
        }
        public int Complete()
        {
            return dbcontext.SaveChanges();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbcontext.Dispose();
            }
        }
    }
}


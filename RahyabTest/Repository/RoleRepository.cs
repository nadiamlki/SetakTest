using Microsoft.AspNetCore.Identity;
using SetakTest.Data;
using SetakTest.Entities;
using SetakTest.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SetakTest.Repository
{
    public class RoleRepository:  GenericRepository<int,AppRole>, IRoleRepository
    {
        public RoleRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }
    }
}

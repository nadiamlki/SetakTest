using Microsoft.AspNetCore.Identity;
using SetakTest.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SetakTest.Repository.Interface
{
   public interface IRoleRepository : IGenericRepository<int, AppRole>
    {
    }
}

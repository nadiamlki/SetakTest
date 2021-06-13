using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SetakTest.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public string RefreshToken { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SetakTest.Controllers.UserController.Contracts.Response
{
    public class EditUserRoleResponse
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string UserName { get; set; }
    }
}

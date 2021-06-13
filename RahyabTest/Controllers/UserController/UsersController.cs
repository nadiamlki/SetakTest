using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SetakTest.Controllers.UserController.Contracts.Request;
using SetakTest.Controllers.UserController.Contracts.Response;
using SetakTest.Data;
using SetakTest.Entities;
using SetakTest.Repository.Interface;
using SetakTest.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SetakTest.Controllers
{
   [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]

    public class UsersController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private IUnitOfWork _unitOfWork;
        private readonly ITokenInfoService _tokenInfoService;


        public UsersController(IUnitOfWork unitOfWork, ApplicationDbContext db, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, ITokenInfoService tokenInfoService)
        {
            _unitOfWork = unitOfWork;
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenInfoService = tokenInfoService;
        }
        [HttpGet("GetAllUsers")]
        public async Task<JsonResultViewModel> GetAllUsers()
        {
            try
            {
                var Userid = _tokenInfoService.GetCurrentUserId();
                if (Userid == 1)
                {
                    var msg = "";
                    var success = false;
                    var status = 0;
                    var res = await _unitOfWork.Users.GetAll();
                    List<GetAllUserResponse> lst = new List<GetAllUserResponse>();
                    foreach (var item in res)
                    {
                        GetAllUserResponse allUserResponse = new GetAllUserResponse();
                        var roleId = _db.UserRoles.Where(x => x.UserId == item.Id).Select(x => x.RoleId).FirstOrDefault();
                        allUserResponse.UserId = item.Id;
                        allUserResponse.Email = item.Email;
                        allUserResponse.PhoneNumber = item.PhoneNumber;
                        allUserResponse.UserName = item.UserName;
                        allUserResponse.RoleId = roleId;
                        if (res.Count() != 0)
                        {
                            msg = "found";
                            success = true;
                            status = 200;
                        }
                        else
                        {
                            success = true;
                            msg = "not found";
                            status = 404;
                        }
                        lst.Add(allUserResponse);
                    }

                    return new JsonResultViewModel()
                    {
                        Message = msg,
                        Success = success,
                        CustomResult = lst,
                        Staus = 200
                    };
                }
                return new JsonResultViewModel()
                {
                    Success = false,
                    Message = "شما قادر به انجام این عملیات نیستید"
                };
            }
            catch (Exception ex)
            {
                return new JsonResultViewModel(ex);
            }
        }
        [HttpPut("EditUserRole/{userid}")]        
        public async Task<JsonResultViewModel> EditUserRole([FromRoute] int userid, [FromBody] EditUserRoleRequest request)
        {
            try
            {
                var Userid = _tokenInfoService.GetCurrentUserId();
                if (Userid == 1)
                {
                    var user = await _userManager.FindByIdAsync(userid.ToString());
                    if (user != null)
                    {
                        var oldRoleId = await _db.UserRoles.Where(x => x.UserId == user.Id).Select(x => x.RoleId).FirstOrDefaultAsync();
                        var oldRoleName = _db.Roles.SingleOrDefault(r => r.Id == oldRoleId).Name;
                        if (request.RoleId != oldRoleId)
                        {
                            var currentRoleId = await _db.UserRoles.Where(x => x.UserId == user.Id).Select(x => x.RoleId).FirstOrDefaultAsync();
                            var b = await _db.Roles.Where(x => x.Id == currentRoleId).Select(x => x.Name).FirstOrDefaultAsync();
                            await _userManager.RemoveFromRoleAsync(user, b);

                            var a = await _db.Roles.Where(x => x.Id == request.RoleId).Select(x => x.Name).FirstOrDefaultAsync();
                            await _userManager.AddToRoleAsync(user, a);
                        }
                        _db.Entry(user).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                    EditUserRoleResponse edit = new EditUserRoleResponse();
                    edit.Id = user.Id;
                    edit.UserName = user.UserName;
                    edit.RoleId = request.RoleId;

                    return new JsonResultViewModel() { Success = true, Staus = 200, Message = "updated successfully", CustomResult = edit };
                }
                return new JsonResultViewModel()
                {
                    Success = false,
                    Message = "شما قادر به انجام این عملیات نیستید",
                    Staus = 401
                };
            }
            catch (Exception ex)
            {
                return new JsonResultViewModel(ex);
            }
        }


    }


}

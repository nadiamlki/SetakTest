using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SetakTest.Controllers.LoginController.Contracts;
using SetakTest.Data;
using SetakTest.Entities;
using SetakTest.Repository.Interface;
using SetakTest.ViewModel;

namespace SetakTest.Controllers.LoginController
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenInfoService _tokenService;
        public LoginController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager
            , ApplicationDbContext db, ITokenInfoService tokenService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _db = db;
            _tokenService = tokenService;
        }

        [HttpPost("Login")]
        public async Task<JsonResultViewModel> Login([FromBody] LoginViewModel model)
        {

            if (!ModelState.IsValid)
                return CreateInvalidUserPassJsonResult();

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return CreateInvalidUserPassJsonResult();

            if (await _userManager.IsLockedOutAsync(user).ConfigureAwait(false))
                return CreateInvalidUserPassJsonResult();

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, true).ConfigureAwait(false);

            if (!result.Succeeded)
                return CreateInvalidUserPassJsonResult();

            var token = await _tokenService.GenerateToken(user).ConfigureAwait(false);

            return new JsonResultViewModel(token);

        }

        [HttpPost("Register")]
        public async Task<JsonResultViewModel> Register([FromBody] RegisterViewModel model)
        {
            try
            {
                var msg = "";
                var success = false;
                var userName = model.Email;
                var user = await _userManager.FindByEmailAsync(userName).ConfigureAwait(false);

                if (user != null)
                {
                    return new JsonResultViewModel
                    {
                        Success = false,
                        Message = "A user with that email address is already exists!",
                        Errors = new List<JsonResultViewModel.ErrorObj> {
                        new JsonResultViewModel.ErrorObj{
                            Exception="A user with that email address is already exists!"
                        }
                    }
                    };

                }
                AppUser u = new AppUser();
                u.PhoneNumber = model.PhoneNumber;
                u.Email = model.Email;
                u.EmailConfirmed = true;
                u.UserName = userName;
                u.LockoutEnabled = true;

                var registerResult = await _userManager.CreateAsync(u, model.Password).ConfigureAwait(false);

                //var roleName = _unitOfWork.Roles.Get(Where: x => x.Id == model.RoleId).Select(x => x.Name);
                var roleName = _db.Roles.Where(x => x.Id == 2).Select(x => x.Name).FirstOrDefault();
                if (!registerResult.Succeeded)
                    return new JsonResultViewModel(registerResult.Errors.Select(r => new KeyValuePair<string, string>(r.Code, r.Description)));

                if (!(await _userManager.AddToRoleAsync(u, roleName).ConfigureAwait(false)).Succeeded)
                    throw new ApplicationException("failed to add role to user");

                RegisterResponse r = new RegisterResponse();
                r.Id = u.Id;
                r.Password = model.Password;
                r.Email = u.Email;
                r.PhoneNumber = u.PhoneNumber;
                r.UserName = userName;
                r.RoleName = roleName;


                return new JsonResultViewModel()
                {
                    Message = msg,
                    Staus=200,
                    Success = true,
                    CustomResult = r

                };

            }
            catch (Exception ex)
            {
                return new JsonResultViewModel(ex);
            }
        }
        private JsonResultViewModel CreateInvalidUserPassJsonResult()
        {
            return new JsonResultViewModel
            {
                Message = "Invalid username and/or password",
                Errors = new List<JsonResultViewModel.ErrorObj> {

                    new JsonResultViewModel.ErrorObj
                    {

                        Exception="Invalid username and/or password"
                    }
                },
                Success = false
            };
        }

        [HttpPost("Refreshtoken")]
        public async Task<JsonResultViewModel> Refreshtoken([FromBody] RefreshTokenViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return new JsonResultViewModel(ModelState);

                var userid = _db.Users.Where(x => x.RefreshToken == model.RefreshToken).Select(x => x.Id).FirstOrDefault();
                var res = await _userManager.FindByIdAsync(userid.ToString());
                var userEmail = _db.Users.AsNoTracking().Where(x => x.RefreshToken == model.RefreshToken).Select(r => r.Email).FirstOrDefault();
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user != null)
                {
                    var token = await _tokenService.GenerateToken(user).ConfigureAwait(false);
                    user.RefreshToken = token.RefreshToken;
                    _db.Users.Update(user);
                    _db.SaveChanges();
                    return new JsonResultViewModel { Success = true,Staus=200, CustomResult = token };
                }
                else
                {
                    return new JsonResultViewModel
                    {
                        Success = false,
                        Staus=404,
                        Message = "Refresh token is invalid",
                        Errors = new List<JsonResultViewModel.ErrorObj> {
                    new JsonResultViewModel.ErrorObj{
                        Exception="Refresh token is invalid"} }
                    };
                }
            }
            catch (Exception ex)
            {
                return new JsonResultViewModel(ex);
            }

        }
    }
}

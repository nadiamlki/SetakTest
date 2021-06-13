using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SetakTest.Entities;
using SetakTest.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SetakTest.Repository.Interface
{
    public interface ITokenInfoService
    {
        Task<TokenViewModel> GenerateToken(AppUser user);
        long GetCurrentUserId();
        TokenValidationParameters GetValidationParameters();
    }
}

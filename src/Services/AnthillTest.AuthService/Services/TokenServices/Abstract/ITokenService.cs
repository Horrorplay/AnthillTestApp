using AnthillTest.AuthService.Entities;
using Microsoft.AspNetCore.Identity;

namespace AnthillTest.AuthService.Services.TokenServices.Abstract;

public interface ITokenService
{
    string CreateToken(AppUser user, List<IdentityRole<long>> role);
}

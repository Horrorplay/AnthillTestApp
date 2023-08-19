using Microsoft.AspNetCore.Identity;

namespace AnthillTest.AuthService.Entities;

public class AppUser : IdentityUser<long>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpireTime { get; set; }
}

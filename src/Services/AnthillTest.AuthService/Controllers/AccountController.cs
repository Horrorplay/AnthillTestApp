using AnthillTest.AuthService.Entities;
using AnthillTest.AuthService.Extensions;
using AnthillTest.AuthService.Infrastructure.Persistence;
using AnthillTest.AuthService.Models.Identity;
using AnthillTest.AuthService.Services.TokenServices.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;

namespace AnthillTest.AuthService.Controllers;
[Route("api/authservice")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly DataContext _context;
    private readonly RoleManager<IdentityRole<long>> _roleManager;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _cache;
    private readonly ILogger<AccountController> _logger;

    public AccountController(ITokenService tokenService, DataContext context, UserManager<AppUser> userManager, IConfiguration configuration,
        IMemoryCache cache, ILogger<AccountController> logger, RoleManager<IdentityRole<long>> roleManager)
    {
        _tokenService = tokenService;
        _context = context;
        _userManager = userManager;
        _configuration = configuration;
        _cache = cache;
        _logger = logger;
        _roleManager = roleManager;
    }
    [HttpPost("Login")]
    public async Task<ActionResult<AuthResponse>> Authenticate([FromBody] AuthRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var cacheKey = $"Login_{request}";
        if (_cache.TryGetValue(cacheKey, out ValueTuple<AuthResponse, string> cache))
        {
            var (cachedAccount, cachedToken) = cache;
            return Ok(new AuthResponse
            {
                Username = cachedAccount.Username,
                Email = cachedAccount.Email,
                Token = cachedToken,
                RefreshToken = cachedAccount.RefreshToken
            });
        }

        var managedUser = await _userManager.FindByEmailAsync(request.Email);

        if (managedUser == null)
        {
            return BadRequest("Bad credentials");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(managedUser, request.Password);

        if (!isPasswordValid)
        {
            return BadRequest("Bad credentials");
        }

        var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);

        if (user is null)
            return Unauthorized();

        var roleIds = await _context.UserRoles.Where(r => r.UserId == user.Id).Select(r => r.RoleId).ToListAsync();
        var roles = _context.Roles.Where(x => roleIds.Contains(x.Id)).ToList();

        var accessToken = _tokenService.CreateToken(user, roles);
        user.RefreshToken = _configuration.GenerateRefreshToken();
        user.RefreshTokenExpireTime = DateTime.UtcNow.AddDays(_configuration.GetSection("Jwt:RefreshTokenValidityInDays").Get<int>());

        await _context.SaveChangesAsync();

        var authResponse = new AuthResponse
        {
            Username = user.UserName!,
            Email = user.Email!,
            Token = accessToken,
            RefreshToken = user.RefreshToken
        };

        _logger.LogInformation($"User_{user.UserName} is authenticated");

        _cache.Set(cacheKey, (authResponse, accessToken));

        return Ok(authResponse);
    }

    [HttpPost("Register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(request);

        var user = new AppUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        if (!result.Succeeded)
            return BadRequest(result);

        var findUser = await _context.Users.FirstOrDefaultAsync(f => f.Email == request.Email)
            ?? throw new Exception($"User {request.Email} not found");

        var roleExists = await _roleManager.RoleExistsAsync(RoleConsts.Member);
        if (!roleExists)
        {
            var newRole = new IdentityRole<long>(RoleConsts.Member);
            var createRoleResult = await _roleManager.CreateAsync(newRole);
            if (!createRoleResult.Succeeded)
            {
                throw new Exception("Роль не установлена");
            }
        }

        await _userManager.AddToRoleAsync(user, RoleConsts.Member);

        var cacheKey = $"Account_{user}";
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(30));
        _cache.Set(cacheKey, cacheEntryOptions);

        _logger.LogInformation($"User_{user.UserName} is registered");

        return await Authenticate(new AuthRequest
        {
            Email = request.Email,
            Password = request.Password,
        });
    }

    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken(TokenModel? tokenModel)
    {
        if (tokenModel is null)
            return BadRequest("Invalid client request");


        var accessToken = tokenModel.AccessToken;
        var refreshToken = tokenModel.RefreshToken;
        var principal = _configuration.GetPrincipalFromExpiredToken(accessToken);

        if (principal == null)
            return BadRequest("Invalid access token or refresh token");

        var username = principal.Identity!.Name;
        var user = await _userManager.FindByNameAsync(username!);

        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpireTime <= DateTime.UtcNow)
            return BadRequest("Invalid access token or refresh token");

        var newAccessToken = _configuration.CreateToken(principal.Claims.ToList());
        var newRefreshToken = _configuration.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return new ObjectResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            refreshToken = newRefreshToken
        });
    }

    [Authorize]
    [HttpPost]
    [Route("logout")]
    public async Task<IActionResult> Logout()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return BadRequest("Invalid user");

        user.RefreshToken = null;
        user.RefreshTokenExpireTime = DateTime.MinValue;
        var cacheKey = $"Login_{user.UserName}";
        _cache.Remove(cacheKey);
        _logger.LogInformation($"User_{user.UserName} is logout");
        await _userManager.UpdateAsync(user);

        return Ok();
    }

    [Authorize]
    [HttpPost]
    [Route("revoke/{username}")]
    public async Task<IActionResult> Revoke(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return BadRequest("Invalid user name");

        user.RefreshToken = null;
        await _userManager.UpdateAsync(user);

        return Ok();
    }

    [Authorize]
    [HttpPost]
    [Route("revoke-all")]
    public async Task<IActionResult> RevokeAll()
    {
        var users = _userManager.Users.ToList();
        foreach (var user in users)
        {
            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);
        }

        return Ok();
    }
}

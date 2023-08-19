using AnthillTest.UserService.DTO;
using AnthillTest.UserService.Entities;
using AnthillTest.UserService.Infrastructure.Persistence;
using AnthillTest.UserService.Services.Abstract;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AnthillTest.UserService.Services.Concrete;

public class UserService : IUserService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly ILogger<UserService> _logger;

    public UserService(DataContext context, IMapper mapper, IMemoryCache cache, ILogger<UserService> logger)
    {
        _context = context;
        _mapper = mapper;
        _cache = cache;
        _logger = logger;
    }

    public async Task<User> CreateUserAsync(UserDto userDto)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
        if (existingUser != null)
        {
            return null;
        }

        var newUser = _mapper.Map<User>(userDto);
        newUser.CreateDate = DateTime.UtcNow;

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        _cache.Remove($"UserByEmail_{userDto.Email}");
        _cache.Remove($"AllUsers");

        _logger.LogInformation($"User_{newUser.NickName} is Created");

        return newUser;
    }

    public async Task<User> UpdateUserAsync(UserDto userDto)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
        if (existingUser == null)
        {
            return null;
        }

        _mapper.Map(userDto, existingUser);

        _context.Users.Update(existingUser);
        await _context.SaveChangesAsync();

        _cache.Remove($"UserByEmail_{userDto.Email}");
        _cache.Remove($"AllUsers");

        _logger.LogInformation($"Updated user: {userDto.NickName}");

        return existingUser;
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        if (_cache.TryGetValue($"UserByEmail_{email}", out User cachedUser))
        {
            return cachedUser;
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user != null)
        {
            _cache.Set($"UserByEmail_{email}", user, TimeSpan.FromMinutes(60));
        }

        return user;
    }

    public async Task<User> GetUserByNickNameAsync(string nickName)
    {
        if (_cache.TryGetValue($"UserByNickName_{nickName}", out User cachedUser))
        {
            return cachedUser;
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.NickName == nickName);
        if (user != null)
        {
            _cache.Set($"UserByNickName_{nickName}", user, TimeSpan.FromMinutes(60));
        }

        return user;
    }

    public async Task<bool> DeleteUserAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return false;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        _cache.Remove($"UserByEmail_{email}");
        _cache.Remove($"AllUsers");

        _logger.LogInformation($"Delete user {user.NickName}");

        return true;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        if (_cache.TryGetValue($"AllUsers", out List<User> cachedUsers))
        {
            return cachedUsers;
        }

        var users = await _context.Users.ToListAsync();
        _cache.Set($"AllUsers", users, TimeSpan.FromMinutes(60));

        return users;
    }
}

using AnthillTest.UserService.DTO;
using AnthillTest.UserService.Entities;

namespace AnthillTest.UserService.Services.Abstract;

public interface IUserService
{
    Task<User> CreateUserAsync(UserDto userDto);
    Task<User> UpdateUserAsync(UserDto userDto);
    Task<User> GetUserByEmailAsync(string email);
    Task<User> GetUserByNickNameAsync(string nickName);
    Task<bool> DeleteUserAsync(string email);
    Task<List<User>> GetAllUsersAsync();
}

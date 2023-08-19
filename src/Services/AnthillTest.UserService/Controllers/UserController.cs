using AnthillTest.UserService.DTO;
using AnthillTest.UserService.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AnthillTest.UserService.Controllers;
[Route("api/UserService")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize]
    [HttpPost("CreateUser")]
    public async Task<IActionResult> Create(UserDto userDto)
    {
        var user = await _userService.CreateUserAsync(userDto);
        return CreatedAtAction(nameof(Get), new { email = user.Email }, user);
    }

    [Authorize]
    [HttpPut("UpdateUser")]
    public async Task<IActionResult> Update(UserDto userDto)
    {
        var updatedUser = await _userService.UpdateUserAsync(userDto);
        if (updatedUser == null)
        {
            return NotFound();
        }
        return Ok(updatedUser);
    }

    [Authorize]
    [HttpGet("GetUserByEmail")]
    public async Task<IActionResult> Get(string email)
    {
        var user = await _userService.GetUserByEmailAsync(email);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [Authorize]
    [HttpGet("GetUserByNickName")]
    public async Task<IActionResult> GetByNickName(string nickName)
    {
        var user = await _userService.GetUserByNickNameAsync(nickName);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [Authorize]
    [HttpDelete("DeleteUser")]
    public async Task<IActionResult> Delete(string email)
    {
        var result = await _userService.DeleteUserAsync(email);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }

    [Authorize]
    [HttpGet("UserList")]
    public async Task<IActionResult> List()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }
}

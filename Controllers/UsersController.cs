using FilmFreakApi.Auth;
using FilmFreakApi.Models;
using FilmFreakApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = UserRoles.Admin)]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers(string role)
    {
        var users = await _userService.GetUsers(role);
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> AddUser(UserAddModel userModel)
    {
        var userExists = await _userService.UserExists(userModel.UserName);
        if (userExists)
        {
            return Conflict("User already registered");
        }
        await _userService.AddUser(userModel, UserRoles.User);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        await _userService.DeleteUser(userId);
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser(UserUpdateModel userModel)
    {
        await _userService.UpdateUser(userModel);
        return Ok();
    }
}
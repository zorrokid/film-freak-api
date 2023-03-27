using FilmFreakApi.Auth;
using FilmFreakApi.Auth.Exceptions;
using FilmFreakApi.Auth.Models;
using FilmFreakApi.Auth.Services;
using FilmFreakApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FilmFreakApi.RestApi.Controllers;

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

    [HttpGet("{role}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserListModel>))]
    public async Task<IActionResult> GetUsers(string role)
    {
        var users = await _userService.GetUsers(role);
        var userListModels = users.Select(u => new UserListModel(
            email: u.Email ?? "",
            userId: u.Id,
            userName: u.UserName ?? ""
        ));
        return Ok(userListModels);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IEnumerable<IdentityError>))]
    public async Task<IActionResult> AddUser(UserAddModel userModel)
    {
        var userExists = await _userService.UserExists(userModel.UserName);
        if (userExists)
        {
            return Conflict("User already registered");
        }
        try
        {
            await _userService.AddUser(userModel, UserRoles.User);
        }
        catch (IdentityException ex)
        {
            return BadRequest(ex.Errors);
        }
        return Ok();
    }

    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var user = await _userService.GetUser(userId);
        if (user == null) return NotFound();
        await _userService.DeleteUser(user);
        return Ok();
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(UserUpdateModel userModel)
    {
        var user = await _userService.GetUser(userModel.UserId);
        if (user == null) return NotFound();
        await _userService.UpdateUser(user, userModel);
        return Ok();
    }
}
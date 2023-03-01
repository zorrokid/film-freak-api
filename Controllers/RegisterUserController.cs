using FilmFreakApi.Auth;
using FilmFreakApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FilmFreakApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegisterUserController : ControllerBase
{
    private readonly ILogger<RegisterUserController> _logger;
    private readonly IUserService _userService;

    public RegisterUserController(
        ILogger<RegisterUserController> logger,
        IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> RegisterUser(RegisterModel model)
    {
        var userExists = await _userService.UserExists(model.UserName);
        if (userExists)
        {
            return Conflict("User already registered");
        }
        await _userService.RegisterUser(model, UserRoles.User);
        return Ok();
    }
}

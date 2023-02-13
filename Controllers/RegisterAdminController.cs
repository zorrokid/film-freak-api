using FilmFreakApi.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilmFreakApi.Controllers;

[Authorize(Roles = UserRoles.Admin)]
[ApiController]
[Route("api/[controller]")]
public class RegisterAdminController : ControllerBase
{
    private readonly ILogger<RegisterAdminController> _logger;
    private readonly IUserService _userService;

    public RegisterAdminController(
        ILogger<RegisterAdminController> logger, 
        IUserService userService)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> RegisterAdminUser(RegisterModel model)
    {
        var userExists = await _userService.UserExists(model.UserName);
        if (userExists)
        {
            return Conflict("User already registered");
        }
        await _userService.RegisterUser(model, UserRoles.Admin);
        return Ok();
    }
}

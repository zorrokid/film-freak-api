using FilmFreakApi.Models;
using FilmFreakApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FilmFreakApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly ILogger<LoginController> _logger;
    private readonly IAuthService _authService;

    public LoginController(
        ILogger<LoginController> logger,
        IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model)
    {
        var tokenRespone = await _authService.LogInUser(model);
        if (tokenRespone == null)
        {
            return Unauthorized();
        }
        return Ok(tokenRespone);
    }
}

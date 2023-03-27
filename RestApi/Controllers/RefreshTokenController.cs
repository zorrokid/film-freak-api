using FilmFreakApi.Auth.Services;
using FilmFreakApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace FilmFreakApi.RestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RefreshTokenController : ControllerBase
{
    private readonly ILogger<RefreshTokenController> _logger;
    private readonly IAuthService _authService;

    public RefreshTokenController(
        ILogger<RefreshTokenController> logger,
        IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    [HttpPost]
    public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
    {
        var tokenResponse = await _authService.RefreshToken(tokenModel);
        if (tokenResponse == null)
        {
            return Unauthorized();
        }
        return Ok(tokenResponse);
    }
}
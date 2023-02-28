using System.IdentityModel.Tokens.Jwt;
using FilmFreakApi.Auth;
using FilmFreakApi.Auth.Services;
using FilmFreakApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilmFreakApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RefreshTokenController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly AuthDbContext _dbContext;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<RefreshTokenController> _logger;

    public RefreshTokenController(
        UserManager<IdentityUser> userManager,
        AuthDbContext dbContext,
        IRefreshTokenService refreshTokenService,
        IJwtTokenService jwtTokenService,
        ILogger<RefreshTokenController> logger)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _refreshTokenService = refreshTokenService;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
    {
        _logger.LogInformation("Refresh token request start.");
        var principal = _jwtTokenService.GetPrincipal(tokenModel.AccessToken);

        if (principal == null || principal.Identity == null)
        {
            throw new Exception("Invalid token refresh request.");
        }

        var userName = principal.Identity.Name;
        if (userName == null)
        {
            throw new Exception("User not found.");
        }
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            throw new Exception($"Did not find user {userName}");
        }
        var refreshToken = await _dbContext.RefreshTokens
            .SingleAsync(t => t.IdentityUserId == user.Id);

        if (refreshToken.Token != tokenModel.RefreshToken)
        {
            _logger.LogInformation("Invalid refresh token");
            return BadRequest("Invalid or expired refresh token");
        }

        if (refreshToken.ExpirationTime < DateTime.UtcNow)
        {
            _logger.LogInformation("Expired refresh token");
            return BadRequest("Invalid or expired refresh token");
        }

        var newAccessToken = await _jwtTokenService.GenerateJwtToken(user);
        var newRefreshToken = await _refreshTokenService.GenerateRefreshToken(user);

        _logger.LogInformation("Refresh token request end.");
        return Ok(new TokenResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            Expiration = newAccessToken.ValidTo,
            RefreshToken = newRefreshToken
        });
    }
}
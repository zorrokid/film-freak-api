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
    private readonly RefreshTokenService _refreshTokenService;
    private readonly JwtTokenService _jwtTokenService;

    public RefreshTokenController(
        UserManager<IdentityUser> userManager,
        AuthDbContext dbContext,
        RefreshTokenService refreshTokenService,
        JwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _refreshTokenService = refreshTokenService;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost]
    public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
    {
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

        if (refreshToken.Token != tokenModel.RefreshToken || refreshToken.ExpirationTime < DateTime.UtcNow)
        {
            return BadRequest("Invalid or expired refresh token");
        }

        var newAccessToken = _jwtTokenService.GenerateJwtToken(principal.Claims.ToList());
        var newRefreshToken = _refreshTokenService.GenerateRefreshToken(user);

        return new ObjectResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            expiration = newAccessToken.ValidTo,
            refreshToken = newRefreshToken
        });
    }
}
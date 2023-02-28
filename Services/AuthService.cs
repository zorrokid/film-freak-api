using System.IdentityModel.Tokens.Jwt;
using FilmFreakApi.Auth;
using FilmFreakApi.Auth.Services;
using FilmFreakApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FilmFreakApi.Services;

public interface IAuthService
{
    Task<TokenResponse?> LogInUser(LoginModel loginModel);
    Task<TokenResponse?> RefreshToken(TokenModel tokenModel);
}

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly AuthDbContext _dbContext;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        ILogger<AuthService> logger,
        UserManager<IdentityUser> userManager,
        IJwtTokenService jwtTokenService,
        IRefreshTokenService refreshTokenService,
        AuthDbContext dbContext)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _refreshTokenService = refreshTokenService;
        _dbContext = dbContext;
        _logger = logger;

    }

    public async Task<TokenResponse?> LogInUser(LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.UserName);
        if (user == null)
        {
            return null;
        }
        var passwordOk = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!passwordOk)
        {
            return null;
        }

        var token = await _jwtTokenService.GenerateJwtToken(user);
        var refreshToken = await _refreshTokenService.GenerateRefreshToken(user);
        return new TokenResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = token.ValidTo,
            RefreshToken = refreshToken
        };
    }

    public async Task<TokenResponse?> RefreshToken(TokenModel tokenModel)
    {
        var principal = _jwtTokenService.GetPrincipal(tokenModel.AccessToken);

        if (principal == null || principal.Identity == null)
        {
            _logger.LogWarning("Invalid token refresh request.");
            return null;
        }

        var userName = principal.Identity.Name;
        if (userName == null)
        {
            _logger.LogWarning("User not found.");
            return null;
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
            return null;
        }

        if (refreshToken.ExpirationTime < DateTime.UtcNow)
        {
            _logger.LogInformation("Expired refresh token");
            return null;
        }

        var newAccessToken = await _jwtTokenService.GenerateJwtToken(user);
        var newRefreshToken = await _refreshTokenService.GenerateRefreshToken(user);

        return new TokenResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            Expiration = newAccessToken.ValidTo,
            RefreshToken = newRefreshToken
        };

    }

}
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using FilmFreakApi.Models;
using FilmFreakApi.Auth.Interfaces;
using Microsoft.Extensions.Logging;

namespace FilmFreakApi.Auth.Services;

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
    private readonly IAuthRepository _authRepository;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        ILogger<AuthService> logger,
        UserManager<IdentityUser> userManager,
        IJwtTokenService jwtTokenService,
        IRefreshTokenService refreshTokenService,
        IAuthRepository authRepository)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _refreshTokenService = refreshTokenService;
        _authRepository = authRepository;
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
        return new TokenResponse(
            token: new JwtSecurityTokenHandler().WriteToken(token),
            expiration: token.ValidTo,
            refreshToken: refreshToken);
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
        var refreshToken = await _authRepository.GetRefreshTokenAsync(user.Id);

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

        return new TokenResponse(
            token: new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            expiration: newAccessToken.ValidTo,
            refreshToken: newRefreshToken);
    }

}
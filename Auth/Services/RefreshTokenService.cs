using FilmFreakApi.Auth.Entities;
using FilmFreakApi.Auth.Interfaces;
using FilmFreakApi.Auth.Options;
using FilmFreakApi.Auth.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FilmFreakApi.Auth.Services;

public interface IRefreshTokenService
{
    Task<string> GenerateRefreshToken(IdentityUser user);
}

public class RefreshTokenService : IRefreshTokenService
{
    private readonly ILogger<RefreshTokenService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IAuthRepository _authRepository;

    public RefreshTokenService(
        ILogger<RefreshTokenService> logger,
        IConfiguration configuration,
        IAuthRepository authRepository)
    {
        _logger = logger;
        _configuration = configuration;
        _authRepository = authRepository;
    }

    public async Task<string> GenerateRefreshToken(IdentityUser user)
    {
        var jwtOptions = _configuration.GetSection(JwtOptions.JWT).Get<JwtOptions>();
        if (jwtOptions == null)
        {
            throw new Exception("JWT options not configured.");
        }
        // delete previous refresh token(s)
        await _authRepository.DeleteRefreshTokensAsync(user.Id);
        var token = RefreshTokenGenerator.Generate();
        await _authRepository.AddRefreshTokenAsync(new RefreshToken
        {
            IdentityUserId = user.Id,
            Token = token,
            ExpirationTime = DateTime.UtcNow.AddHours(jwtOptions.RefreshTokenExpirationInHours),
        });
        return token;
    }

}
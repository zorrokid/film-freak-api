using FilmFreakApi.Auth.Entities;
using Microsoft.AspNetCore.Identity;

namespace FilmFreakApi.Auth.Services;

public interface IRefreshTokenService
{
    Task<string> GenerateRefreshToken(IdentityUser user);
}

public class RefreshTokenService : IRefreshTokenService
{
    private readonly ILogger<RefreshTokenService> _logger;
    private readonly AuthDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public RefreshTokenService(
        ILogger<RefreshTokenService> logger,
        AuthDbContext dbContext,
        IConfiguration configuration)
    {
        _logger = logger;
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public async Task<string> GenerateRefreshToken(IdentityUser user)
    {
        var token = RefreshTokenGenerator.Generate();
        var jwtOptions = _configuration.GetSection(JwtOptions.JWT).Get<JwtOptions>();
        if (jwtOptions == null)
        {
            throw new Exception("JWT options not configured.");
        }

        _dbContext.RefreshTokens.Add(new RefreshToken
        {
            IdentityUserId = user.Id,
            Token = token,
            ExpirationTime = DateTime.UtcNow.AddHours(jwtOptions.RefreshTokenExpirationInHours),
        });

        await _dbContext.SaveChangesAsync();
        return token;
    }

}
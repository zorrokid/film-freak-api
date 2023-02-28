using FilmFreakApi.Auth.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        var jwtOptions = _configuration.GetSection(JwtOptions.JWT).Get<JwtOptions>();
        if (jwtOptions == null)
        {
            throw new Exception("JWT options not configured.");
        }

        // delete previous refresh token(s)
        var oldRefreshTokens = await _dbContext.RefreshTokens
            .Where(t => t.IdentityUserId == user.Id).ToListAsync();

        if (oldRefreshTokens.Any())
        {
            _dbContext.RemoveRange(oldRefreshTokens);
        }

        var token = RefreshTokenGenerator.Generate();
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
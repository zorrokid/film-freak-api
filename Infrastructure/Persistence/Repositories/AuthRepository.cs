using FilmFreakApi.Auth.Entities;
using FilmFreakApi.Auth.Interfaces;
using FilmFreakApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FilmFreakApi.Infrasturcture.Persistence.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly AuthDbContext _dbContext;

    public AuthRepository(AuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddRefreshTokenAsync(RefreshToken token)
    {
        _dbContext.RefreshTokens.Add(token);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteRefreshTokensAsync(string userId)
    {
        // delete previous refresh token(s)
        var oldRefreshTokens = await _dbContext.RefreshTokens
            .Where(t => t.IdentityUserId == userId).ToListAsync();

        if (oldRefreshTokens.Any())
        {
            _dbContext.RemoveRange(oldRefreshTokens);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<RefreshToken> GetRefreshTokenAsync(string userId)
    {
        return await _dbContext.RefreshTokens
            .SingleAsync(t => t.IdentityUserId == userId);
    }
}
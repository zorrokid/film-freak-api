using FilmFreakApi.Auth.Entities;

namespace FilmFreakApi.Auth.Interfaces;

public interface IAuthRepository
{
    Task<RefreshToken> GetRefreshTokenAsync(string userId);
    Task DeleteRefreshTokensAsync(string userId);
    Task AddRefreshTokenAsync(RefreshToken token);
}
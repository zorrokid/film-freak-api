
namespace FilmFreakApi.Auth.Entities;

public class RefreshToken
{
    public long RefreshTokenId { get; set; }
    public string IdentityUserId { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;

    public DateTime ExpirationTime { get; set; }
}

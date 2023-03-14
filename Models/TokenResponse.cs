namespace FilmFreakApi.Models;

public class TokenResponse
{
    public TokenResponse(string token, string refreshToken, DateTime expiration)
    {
        Token = token;
        RefreshToken = refreshToken;
        Expiration = expiration;
    }
    public string Token { get; }
    public string RefreshToken { get; }
    public DateTime Expiration { get; }
}
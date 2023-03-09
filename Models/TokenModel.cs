namespace FilmFreakApi.Models;

public class TokenModel
{
    public TokenModel(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    public string AccessToken { get; }
    public string RefreshToken { get; }
}
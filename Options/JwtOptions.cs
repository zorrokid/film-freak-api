public class JwtOptions
{
    public const string JWT = "JWT";
    public string ValidAudience { get; set; } = string.Empty;
    public string ValidIssuer { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    public int ExpirationInHours { get; set; } = 1;
    public int RefreshTokenExpirationInHours { get; set; } = 24;

}
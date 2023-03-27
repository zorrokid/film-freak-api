namespace FilmFreakApi.Auth.Options;
public class AdminCredentialsOptions
{
    public const string AdminCredentials = "AdminCredentials";
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
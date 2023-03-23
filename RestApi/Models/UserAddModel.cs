namespace FilmFreakApi.Models;

public class UserAddModel
{
    public UserAddModel(string userName, string password, string email, string role)
    {
        UserName = userName;
        Password = password;
        Email = email;
        Role = role;
    }

    public string UserName { get; }
    public string Password { get; }
    public string Email { get; }
    public string Role { get; }
}
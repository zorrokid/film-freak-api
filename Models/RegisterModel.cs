namespace FilmFreakApi.Models;

public class RegisterModel
{
    public RegisterModel(string userName, string password, string email)
    {
        this.UserName = userName;
        this.Password = password;
        this.Email = email;
    }
    public string UserName { get; }
    public string Password { get; }
    public string Email { get; }
}
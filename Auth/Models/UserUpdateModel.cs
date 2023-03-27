namespace FilmFreakApi.Auth.Models;

public class UserUpdateModel
{
    public UserUpdateModel(string userId, string email)
    {
        UserId = userId;
        Email = email;
    }

    public string UserId { get; }
    public string Email { get; }
}
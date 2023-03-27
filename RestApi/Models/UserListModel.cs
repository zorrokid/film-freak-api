namespace FilmFreakApi.Models;

public class UserListModel
{
    public UserListModel(string userId, string userName, string email)
    {
        UserId = userId;
        Email = email;
        UserName = userName;
    }

    public string UserId { get; }
    public string UserName { get; }
    public string Email { get; }

}
namespace FilmFreakApi.Models;

public class LoggedInUserModel
{
    public LoggedInUserModel(string userId, string userName, bool isAdmin)
    {
        UserId = userId;
        UserName = userName;
        IsAdmin = isAdmin;
    }

    public string UserId { get; }
    public string UserName { get; }
    public bool IsAdmin { get; }
}
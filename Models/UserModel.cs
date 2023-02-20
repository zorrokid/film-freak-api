public class UserModel
{
    public UserModel(string userId, string userName, bool isAdmin)
    {
        UserId = userId;
        UserName = userName;
        IsAdmin = isAdmin;
    }

    public string UserId { get; set; } = default;
    public string UserName { get; set; } = string.Empty;
    public bool IsAdmin { get; set; } = false;
}
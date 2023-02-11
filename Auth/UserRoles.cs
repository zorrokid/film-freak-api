namespace FilmFreakApi.Auth;

public static class UserRoles
{
    public const string Admin = "Administrator";
    public const string User = "User";

    public static List<string> UserRoleNames = new List<string> { Admin, User };
}
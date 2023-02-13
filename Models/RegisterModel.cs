public class RegisterModel
{
    public RegisterModel(string userName, string password, string email)
    {
        this.UserName = userName;
        this.Password = password;
        this.Email = email;
    }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Email { get; set;}
}
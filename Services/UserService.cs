using Microsoft.AspNetCore.Identity;

namespace FilmFreakApi.Services;

public interface IUserService
{
    Task<bool> UserExists(string userName);
    Task RegisterUser(RegisterModel model, string role);
}

public class UserService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<UserService> _logger;

    public UserService(ILogger<UserService> logger,
        UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
        _logger = logger;
    }


    public async Task RegisterUser(RegisterModel model, string role)
    {
        var user = new IdentityUser
        {
            Email = model.Email,
            UserName = model.UserName,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            HandleErrors(result.Errors);
            throw new Exception("Failed creating user");
        }

        result = await _userManager.AddToRoleAsync(user, role);
        if (!result.Succeeded)
        {
            HandleErrors(result.Errors);
        }
    }

    public async Task<bool> UserExists(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        return user != null;
    }

    private void HandleErrors(IEnumerable<IdentityError> errors)
    {
        foreach (var error in errors)
        {
            _logger.LogError($"Error {error.Code}: {error.Description}");
        }
    }
}
using FilmFreakApi.Auth.Exceptions;
using FilmFreakApi.Models;
using Microsoft.AspNetCore.Identity;

namespace FilmFreakApi.Services;

public interface IUserService
{
    Task<bool> UserExists(string userName);
    Task AddUser(UserAddModel model, string role);
    Task<IList<IdentityUser>> GetUsers(string role);
    Task<IdentityUser?> GetUser(string userId);
    Task UpdateUser(IdentityUser user, UserUpdateModel userModel);
    Task DeleteUser(IdentityUser user);
    Task ChangePassword(string userId, string password);
    Task AddRole(string userId, string role);
    Task RemoveRole(string userId, string role);
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

    public async Task DeleteUser(IdentityUser userToDelete)
    {
        _logger.LogInformation("Trying to delete user with id {userId}", userToDelete.Id);
        var identityResult = await _userManager.DeleteAsync(userToDelete);
        if (!identityResult.Succeeded)
        {
            throw new IdentityException("Failed deleting user", identityResult.Errors);
        }
        _logger.LogInformation("Deleted user with id {userId}", userToDelete.Id);
    }

    public async Task<IList<IdentityUser>> GetUsers(string role)
    {
        return await _userManager.GetUsersInRoleAsync(role);
    }

    public async Task AddUser(UserAddModel model, string role)
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
            throw new IdentityException("Failed creating user", result.Errors);
        }

        result = await _userManager.AddToRoleAsync(user, role);
        if (!result.Succeeded)
        {
            HandleErrors(result.Errors);
            throw new IdentityException("Failed adding role to user", result.Errors);
        }
    }

    public async Task UpdateUser(IdentityUser userToUpdate, UserUpdateModel userModel)
    {
        _logger.LogInformation("Trying to update user with id {userId}", userModel.UserId);
        userToUpdate.Email = userModel.Email;
        var identityResult = await _userManager.UpdateAsync(userToUpdate);
        if (!identityResult.Succeeded)
        {
            throw new IdentityException("Failed updating user", identityResult.Errors);
        }
        _logger.LogInformation("Deleted user with id {userId}", userModel.UserId);
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

    public Task ChangePassword(string userId, string password)
    {
        throw new NotImplementedException();
    }

    public Task AddRole(string userId, string role)
    {
        throw new NotImplementedException();
    }

    public Task RemoveRole(string userId, string role)
    {
        throw new NotImplementedException();
    }

    public async Task<IdentityUser?> GetUser(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }
}
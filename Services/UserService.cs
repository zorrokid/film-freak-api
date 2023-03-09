using FilmFreakApi.Auth.Exceptions;
using FilmFreakApi.Models;
using Microsoft.AspNetCore.Identity;

namespace FilmFreakApi.Services;

public interface IUserService
{
    Task<bool> UserExists(string userName);
    Task AddUser(UserAddModel model, string role);
    Task<IList<IdentityUser>> GetUsers(string role);
    Task UpdateUser(UserUpdateModel userModel);
    Task DeleteUser(string userId);
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

    public async Task DeleteUser(string userId)
    {
        _logger.LogInformation("Trying to delete user with id {userId}", userId);
        var userToDelete = await _userManager.FindByIdAsync(userId);
        if (userToDelete == null)
        {
            throw new IdentityException("Trying to delete non existent user.");
        }
        var identityResult = await _userManager.DeleteAsync(userToDelete);
        if (!identityResult.Succeeded)
        {
            throw new IdentityException("Failed deleting user", identityResult.Errors);
        }
        _logger.LogInformation("Deleted user with id {userId}", userId);
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
            throw new Exception("Failed creating user");
        }

        result = await _userManager.AddToRoleAsync(user, role);
        if (!result.Succeeded)
        {
            HandleErrors(result.Errors);
        }
    }

    public async Task UpdateUser(UserUpdateModel userModel)
    {
        _logger.LogInformation("Trying to update user with id {userId}", userModel.UserId);
        var userToUpdate = await _userManager.FindByIdAsync(userModel.UserId);
        if (userToUpdate == null)
        {
            throw new IdentityException("Trying to update non existent user.");
        }
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
}
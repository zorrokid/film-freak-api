
using FilmFreakApi.Auth.Options;
using FilmFreakApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FilmFreakApi.Auth.Services;

public interface IAuthDbInitializationService
{
    Task Initialize(AdminCredentialsOptions options);
}

public class AuthDbInitializationService : IAuthDbInitializationService
{
    private readonly ILogger<AuthDbInitializationService> _logger;
    private readonly AuthDbContext _dbContext;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;

    public AuthDbInitializationService(
        ILogger<AuthDbInitializationService> logger,
        AuthDbContext dbContext,
        RoleManager<IdentityRole> roleManager,
        UserManager<IdentityUser> userManager)
    {
        _logger = logger;
        _dbContext = dbContext;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task Initialize(AdminCredentialsOptions options)
    {
        _logger.LogInformation("Start initializing AuthDb.");
        _logger.LogInformation("Running migrations.");
        await _dbContext.Database.MigrateAsync();
        _logger.LogInformation("Initializing roles.");
        await InitializeRoles();
        _logger.LogInformation("Initializing admin user.");
        await InitializeAdminUser(options);
        _logger.LogInformation("Finished initializing AuthDb.");
    }

    private async Task InitializeRoles()
    {
        foreach (var userRoleName in UserRoles.UserRoleNames)
        {
            var roleExists = await _roleManager.RoleExistsAsync(userRoleName);
            if (roleExists)
            {
                _logger.LogInformation($"Role {userRoleName} already exists.");
            }
            if (!roleExists)
            {
                _logger.LogInformation($"Creating role {userRoleName}.");
                var result = await _roleManager.CreateAsync(new IdentityRole(userRoleName));
                if (!result.Succeeded)
                {
                    HandleErrors(result.Errors);
                    throw new Exception($"Failed adding IdentityRole {userRoleName}.");
                }
            }
        }
    }

    private async Task InitializeAdminUser(AdminCredentialsOptions options)
    {
        _logger.LogInformation($"Creating admin user {options.UserName}.");
        var admin = await _userManager.FindByNameAsync(options.UserName);
        if (admin != null)
        {
            _logger.LogTrace("Admin already created.");
            return;
        }

        admin = new IdentityUser
        {
            UserName = options.UserName,
            Email = options.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(admin, options.Password);
        if (!result.Succeeded)
        {
            HandleErrors(result.Errors);
            throw new Exception("Failed creating admin user.");
        }

        result = await _userManager.AddToRoleAsync(admin, UserRoles.Admin);
        if (!result.Succeeded)
        {
            HandleErrors(result.Errors);
            throw new Exception("Failed adding admin role.");
        }
    }

    private void HandleErrors(IEnumerable<IdentityError> errors)
    {
        foreach (var error in errors)
        {
            _logger.LogError($"Error {error.Code}: {error.Description}");
        }
    }
}
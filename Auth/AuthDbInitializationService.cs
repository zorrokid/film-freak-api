
using FilmFreakApi.Auth;
using Microsoft.AspNetCore.Identity;

public interface IAuthDbInitializationService
{
    Task Initialize();
}

public class AuthDbInitializationService : IAuthDbInitializationService // IHostedService
{
    private readonly ILogger<AuthDbInitializationService> _logger;
    private readonly AuthDbContext _dbContext;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _config;

    public AuthDbInitializationService(
        ILogger<AuthDbInitializationService> logger, 
        AuthDbContext dbContext, 
        RoleManager<IdentityRole> roleManager, 
        UserManager<IdentityUser> userManager, 
        IConfiguration config)
    {
        _logger = logger;
        _dbContext = dbContext;
        _roleManager = roleManager;
        _userManager = userManager;
        _config = config;
    }

    public async Task Initialize()
    {
        _logger.LogInformation("Start initializing AuthDb."); 
        _dbContext.Database.EnsureCreated();
        await InitializeRoles();
        await InitializeAdminUser();
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

    private async Task InitializeAdminUser()
    {
        var userName = _config["AdminCredentials:UserName"];
        var password = _config["AdminCredentials:Password"];
        var email = _config["AdminCredentials:Email"];
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
        {
            throw new Exception("One or more of the admin credentials was missing.");
        }
        _logger.LogInformation($"Creating admin user {userName}.");
        var admin = await _userManager.FindByNameAsync(userName);
        if (admin != null)
        {
            _logger.LogTrace("Admin already created.");
            return;
        }

        admin = new IdentityUser
        {
            UserName = userName,
            Email = email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(admin, password);
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
        foreach(var error in errors)
        {
            _logger.LogError($"Error {error.Code}: {error.Description}");
        }
    }
}
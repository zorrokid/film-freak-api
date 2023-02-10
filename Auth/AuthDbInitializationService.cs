
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
       _logger.LogInformation("Start initializing AuthDb"); 
       var roleExists = await _roleManager.RoleExistsAsync("Administrator");
    }
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FilmFreakApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LoginController> _logger;

    public LoginController(
        UserManager<IdentityUser> userManager, 
        RoleManager<IdentityRole> roleManager, 
        IConfiguration configuration, 
        ILogger<LoginController> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.UserName);
        if (user == null)
        {
            return Unauthorized();
        }
        var passwordOk = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!passwordOk)
        {
            return Unauthorized();
        }
        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var userRoles = await _userManager.GetRolesAsync(user);

        foreach(var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }
        var validIssuer = _configuration["JWT:ValidIssuer"];
        var validAudience = _configuration["JWT:ValidAudience"];
        var secret = _configuration["JWT:Secret"];
        if (string.IsNullOrEmpty(validIssuer) || string.IsNullOrEmpty(validAudience) || string.IsNullOrEmpty(secret))
        {
            throw new Exception("One or more of the JWT token settings are missing.");
        }
        _logger.LogInformation($"Secret: {secret}");
        var token = GetToken(authClaims, validIssuer, validAudience, secret);
        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo
        });
    }

    private JwtSecurityToken GetToken(
        List<Claim> authClaims, 
        string validIssuer, 
        string validAudience, 
        string secret)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        return new JwtSecurityToken(
            issuer: validIssuer,
            audience: validAudience,
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
    }
}

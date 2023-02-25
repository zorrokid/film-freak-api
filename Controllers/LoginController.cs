using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FilmFreakApi.Auth.Services;
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
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenService _refreshTokenService;

    public LoginController(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        ILogger<LoginController> logger,
        IJwtTokenService jwtTokenService,
        IRefreshTokenService refreshTokenService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _logger = logger;
        _jwtTokenService = jwtTokenService;
        _refreshTokenService = refreshTokenService;
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
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var userRoles = await _userManager.GetRolesAsync(user);

        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var token = _jwtTokenService.GenerateJwtToken(authClaims);
        var refreshToken = _refreshTokenService.GenerateRefreshToken(user);
        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo,
            refreshToken = refreshToken
        });
    }

    private JwtSecurityToken GenerateJwtToken(
        List<Claim> authClaims,
        JwtOptions options
        )
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Secret));

        return new JwtSecurityToken(
            issuer: options.ValidIssuer,
            audience: options.ValidAudience,
            expires: DateTime.Now.AddHours(options.ExpirationInHours),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
    }


}

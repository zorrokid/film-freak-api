using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FilmFreakApi.Auth.Services;
using FilmFreakApi.Models;
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

        var token = await _jwtTokenService.GenerateJwtToken(user);
        var refreshToken = await _refreshTokenService.GenerateRefreshToken(user);
        return Ok(new TokenResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = token.ValidTo,
            RefreshToken = refreshToken
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
            expires: DateTime.UtcNow.AddMinutes(options.ExpirationInMinutes),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
    }
}

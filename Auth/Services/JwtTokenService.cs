using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FilmFreakApi.Auth.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace FilmFreakApi.Auth.Services;

public interface IJwtTokenService
{
    Task<JwtSecurityToken> GenerateJwtToken(IdentityUser user);
    ClaimsPrincipal? GetPrincipal(string token);
}

public class JwtTokenService : IJwtTokenService
{
    private readonly ILogger<JwtTokenService> _logger;
    private readonly IConfiguration _configuration;
    private readonly UserManager<IdentityUser> _userManager;

    public JwtTokenService(
        ILogger<JwtTokenService> logger,
        IConfiguration configuration,
        UserManager<IdentityUser> userManager)
    {
        _logger = logger;
        _configuration = configuration;
        _userManager = userManager;
    }

    public async Task<JwtSecurityToken> GenerateJwtToken(IdentityUser user)
    {
        var options = _configuration.GetSection(JwtOptions.JWT).Get<JwtOptions>();
        if (options == null)
        {
            throw new Exception("JWT options not configured.");
        }

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Secret));

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

        return new JwtSecurityToken(
            issuer: options.ValidIssuer,
            audience: options.ValidAudience,
            expires: DateTime.UtcNow.AddMinutes(options.ExpirationInMinutes),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
    }

    public ClaimsPrincipal? GetPrincipal(string token)
    {
        var options = _configuration.GetSection(JwtOptions.JWT).Get<JwtOptions>();
        if (options == null)
        {
            throw new Exception("JWT options not configured.");
        }

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Secret));

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = authSigningKey,
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        // TODO ensure needed checks for token
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }
}
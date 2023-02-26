using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace FilmFreakApi.Auth.Services;

public interface IJwtTokenService
{
    JwtSecurityToken GenerateJwtToken(List<Claim> authClaims);
    ClaimsPrincipal? GetPrincipal(string token);
}

public class JwtTokenService : IJwtTokenService
{
    private readonly ILogger<JwtTokenService> _logger;
    private readonly IConfiguration _configuration;

    public JwtTokenService(
        ILogger<JwtTokenService> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public JwtSecurityToken GenerateJwtToken(List<Claim> authClaims)
    {
        var options = _configuration.GetSection(JwtOptions.JWT).Get<JwtOptions>();
        if (options == null)
        {
            throw new Exception("JWT options not configured.");
        }

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Secret));

        return new JwtSecurityToken(
            issuer: options.ValidIssuer,
            audience: options.ValidAudience,
            expires: DateTime.Now.AddHours(options.ExpirationInHours),
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
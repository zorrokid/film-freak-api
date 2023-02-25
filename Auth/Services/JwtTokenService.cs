using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace FilmFreakApi.Auth.Services;

public interface IJwtTokenService
{
    JwtSecurityToken GenerateJwtToken(List<Claim> authClaims);
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
}
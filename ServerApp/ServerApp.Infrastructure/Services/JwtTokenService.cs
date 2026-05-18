namespace ServerApp.Infrastructure.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ServerApp.Domain.Entities;
using ServerApp.Application.Services;

// Implementation of IJwtTokenService from Application layer

public class JwtTokenService : IJwtTokenService
{
    private readonly string _secretKey;
    private readonly int _expiryMinutes;

    public JwtTokenService(IConfiguration configuration)
    {
        _secretKey = configuration["Admin:JwtSecretKey"]
            ?? throw new ArgumentNullException("Admin:JwtSecretKey is not configured");
        if (!int.TryParse(configuration["Admin:JwtExpiryMinutes"], out _expiryMinutes))
        {
            _expiryMinutes = 60;
        }
    }

    public string GenerateToken(AdminUser adminUser)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, adminUser.Id.ToString()),
            new Claim(ClaimTypes.Email, adminUser.Email.Value),
            new Claim(ClaimTypes.Name, adminUser.DisplayName.Value)
        };

        var token = new JwtSecurityToken(
            expires: DateTime.UtcNow.AddMinutes(_expiryMinutes),
            signingCredentials: credentials,
            claims: claims);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var key = Encoding.UTF8.GetBytes(_secretKey);

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }
}

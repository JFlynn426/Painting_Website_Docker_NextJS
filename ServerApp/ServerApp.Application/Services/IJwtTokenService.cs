namespace ServerApp.Application.Services;

using System.Security.Claims;
using ServerApp.Domain.Entities;

public interface IJwtTokenService
{
    string GenerateToken(AdminUser adminUser);
    ClaimsPrincipal? ValidateToken(string token);
}

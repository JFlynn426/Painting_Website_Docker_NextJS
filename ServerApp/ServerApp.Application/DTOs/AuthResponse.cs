namespace ServerApp.Application.DTOs;

public class AuthResponse
{
    public string Token { get; init; } = string.Empty;
    public AdminUserDto AdminUser { get; init; } = default!;

    public AuthResponse(string token, AdminUserDto adminUser)
    {
        Token = token;
        AdminUser = adminUser;
    }
}

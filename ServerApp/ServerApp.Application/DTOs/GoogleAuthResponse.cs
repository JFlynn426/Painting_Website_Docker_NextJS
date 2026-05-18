namespace ServerApp.Application.DTOs;

public class GoogleAuthResponse
{
    public string Token { get; init; } = string.Empty;
    public AdminUserDto AdminUser { get; init; } = default!;

    public GoogleAuthResponse(string token, AdminUserDto adminUser)
    {
        Token = token;
        AdminUser = adminUser;
    }
}

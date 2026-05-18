namespace ServerApp.Application.DTOs;

public class AdminUserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string? PictureUrl { get; init; }
    public DateTime LastLoginAt { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsActive { get; init; }
}

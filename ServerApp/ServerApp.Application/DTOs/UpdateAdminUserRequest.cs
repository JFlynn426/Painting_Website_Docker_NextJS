namespace ServerApp.Application.DTOs;

/// <summary>
/// Request DTO for updating an admin user.
/// </summary>
public class UpdateAdminUserRequest
{
    public string? DisplayName { get; set; }
    public string? PictureUrl { get; set; }
    public bool? IsActive { get; set; }
}

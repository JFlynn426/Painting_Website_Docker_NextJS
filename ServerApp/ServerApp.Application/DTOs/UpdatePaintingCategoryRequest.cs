namespace ServerApp.Application.DTOs;

/// <summary>
/// Request DTO for updating a painting category.
/// </summary>
public class UpdatePaintingCategoryRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}

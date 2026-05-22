namespace ServerApp.Application.DTOs;

/// <summary>
/// Request DTO for bulk reassigning paintings to different categories.
/// Key = Painting ID, Value = Target Category ID.
/// </summary>
public class ReassignPaintingsRequest
{
    public Dictionary<Guid, Guid> PaintingIdToCategoryId { get; set; } = new();
}

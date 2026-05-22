namespace ServerApp.Application.DTOs;

/// <summary>
/// Request DTO for updating a painting.
/// </summary>
public class UpdatePaintingRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? Slug { get; set; }
    public string? CategoryId { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? Depth { get; set; }
    public int? Year { get; set; }
    public decimal? Price { get; set; }
    public bool? IsAvailable { get; set; }
    public bool? IsNew { get; set; }
    public bool? IsCarouselPainting { get; set; }
}

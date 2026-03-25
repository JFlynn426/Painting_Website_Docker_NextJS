namespace ServerApp.Application.DTOs;

public class PaintingDto
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public string? ThumbnailUrl { get; init; }
    public string CategorySlug { get; init; } = string.Empty;
    public decimal? Width { get; init; }
    public decimal? Height { get; init; }
    public decimal? Depth { get; init; }
    public int? Year { get; init; }
    public decimal? Price { get; init; }
    public bool IsAvailable { get; init; }
}
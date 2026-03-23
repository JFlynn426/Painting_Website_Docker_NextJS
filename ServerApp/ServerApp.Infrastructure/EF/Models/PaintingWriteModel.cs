namespace ServerApp.Infrastructure.EF.Models;

public class PaintingWriteModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string CategorySlug { get; set; } = string.Empty;
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }
    public decimal? Depth { get; set; }
    public int? Year { get; set; }
    public decimal? Price { get; set; }
    public bool IsAvailable { get; set; }

    // Navigation property for the category this painting belongs to
    public PaintingCategoryWriteModel? Category { get; set; }
    public Guid? CategoryId { get; set; }
}
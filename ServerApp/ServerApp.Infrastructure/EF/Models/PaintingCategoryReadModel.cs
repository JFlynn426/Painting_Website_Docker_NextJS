namespace ServerApp.Infrastructure.EF.Models;

public class PaintingCategoryReadModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation property for Paintings in this category
    public ICollection<PaintingReadModel> Paintings { get; set; } = new List<PaintingReadModel>();
}
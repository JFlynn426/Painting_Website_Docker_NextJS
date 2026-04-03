namespace ServerApp.Infrastructure.SeedData;

public static class PaintingsSeedData
{
    public static readonly IEnumerable<PaintingSeed> Seascapes = SeascapesSeedData.Seascapes;
    public static readonly IEnumerable<PaintingSeed> Animals = AnimalsSeedData.Animals;
    public static readonly IEnumerable<PaintingSeed> LandscapesAndCityscapes = LandscapesAndCityscapesSeedData.LandscapesAndCityscapes;
    public static readonly IEnumerable<PaintingSeed> Flowers = FlowersSeedData.Flowers;

    public static readonly IEnumerable<PaintingSeed> Paintings =
        Seascapes.Concat(Animals).Concat(LandscapesAndCityscapes).Concat(Flowers);
}
public class PaintingSeed
{
    public required string Title { get; set; }
    public required string Slug { get; set; }
    public string? Description { get; set; }
    public required string ImageUrl { get; set; }
    public required string CategorySlug { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? Depth { get; set; }
    public int? Year { get; set; }
    public decimal? Price { get; set; }
    public bool IsAvailable { get; set; } = true;
    public bool IsNew { get; set; } = false;
    public string? ThumbnailUrl { get; set; }
}
namespace ServerApp.Infrastructure.SeedData;

/// <summary>
/// Seed data for the PaintingCategories table.
/// 
/// Table Schema:
/// - Id (uniqueidentifier, PK, ValueGeneratedOnAdd)
/// - Name (nvarchar(100), Required)
/// - Slug (nvarchar(100), Required, Unique Index)
/// - Description (nvarchar(max), Optional)
/// 
/// This data matches the client-side models in clientapp/src/app/models/paintingCategories.ts
/// to ensure consistency between frontend and backend data.
/// </summary>
public static class PaintingCategoriesSeedData
{
    public static readonly List<PaintingCategorySeed> Categories = new()
    {
        new PaintingCategorySeed
        {
            Name = "Landscapes & Cityscapes",
            Slug = "landscapes-and-cityscapes",
            Description = "Beautiful landscapes and urban cityscapes"
        },
        new PaintingCategorySeed
        {
            Name = "Seascapes",
            Slug = "seascapes",
            Description = "Ocean and coastal scenes"
        },
        new PaintingCategorySeed
        {
            Name = "Animals & People",
            Slug = "animals-and-people",
            Description = "Portraits and animal paintings"
        },
        new PaintingCategorySeed
        {
            Name = "Flowers",
            Slug = "flowers",
            Description = "Botanical and floral compositions"
        },
        new PaintingCategorySeed
        {
            Name = "New Paintings",
            Slug = "new-paintings",
            Description = "Discover our latest additions to the collection"
        }
    };
}

/// <summary>
/// Represents seed data for a painting category.
/// Matches the PaintingCategoryDto structure.
/// </summary>
public class PaintingCategorySeed
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
}
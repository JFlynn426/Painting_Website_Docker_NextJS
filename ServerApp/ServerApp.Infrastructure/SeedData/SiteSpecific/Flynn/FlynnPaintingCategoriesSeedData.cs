using ServerApp.Infrastructure.SeedData;

namespace ServerApp.Infrastructure.SeedData.SiteSpecific.Flynn;

/// <summary>
/// Seed data for the PaintingCategories table for the Flynn (flynnart.com) site.
/// Flynn has two artwork categories: Ceramics (IMG_* photos) and Paintings (oil paintings).
/// The "New Artwork" section is not a category — it is driven by the IsNew flag on each
/// painting, so no new-paintings category is seeded here.
/// </summary>
public static class FlynnPaintingCategoriesSeedData
{
    public static readonly List<PaintingCategorySeed> Categories = new()
    {
        new PaintingCategorySeed
        {
            Name = "Paintings",
            Slug = "paintings"
        },
        new PaintingCategorySeed
        {
            Name = "Ceramics",
            Slug = "ceramics"
        }
    };
}

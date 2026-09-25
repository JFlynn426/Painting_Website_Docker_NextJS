using ServerApp.Infrastructure.SeedData;

namespace ServerApp.Infrastructure.SeedData.SiteSpecific.Flynn;

/// <summary>
/// Seed data for the PaintingCategories table for the Flynn (flynnart.com) site.
/// Flynn has two artwork categories: Ceramics (IMG_* photos) and Paintings (oil paintings).
/// Also includes the "New Artwork" category (slug: new-paintings) for consistency with GG;
/// the navbar label is driven by the NEXT_PUBLIC_NAVBAR_ARTWORK_LABEL_PLURAL env var.
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
        },
        new PaintingCategorySeed
        {
            Name = "New Artwork",
            Slug = "new-paintings"
        }
    };
}

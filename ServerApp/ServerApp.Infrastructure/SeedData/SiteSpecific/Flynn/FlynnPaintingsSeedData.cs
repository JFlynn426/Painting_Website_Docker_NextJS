using ServerApp.Infrastructure.SeedData;

namespace ServerApp.Infrastructure.SeedData.SiteSpecific.Flynn;

/// <summary>
/// Seed data for the Paintings table for the Flynn (flynnart.com) site.
/// Contains 7 oil paintings and 8 ceramic pieces.
/// 
/// Image URLs follow the pattern:
///   - High-res: /images/high-res/{filename}.jpeg
///   - Thumbnail: /images/thumbnail/{filename}_.jpg
/// 
/// Ceramics have no dimensions (Width/Height/Depth are null).
/// Paintings have dimensions derived from their filenames where present.
/// </summary>
public static class FlynnPaintingsSeedData
{
    public static readonly IEnumerable<PaintingSeed> Paintings = new[]
    {
        // === Paintings (7) ===
        new PaintingSeed
        {
            Title = "B&W Bird",
            Slug = "b-w-bird",
            Description = "Oil on canvas.",
            ImageUrl = "/images/high-res/B_W_Bird_12_16.jpeg",
            ThumbnailUrl = "/images/thumbnail/B_W_Bird_12x16_.jpg",
            CategorySlug = "paintings",
            Width = 12,
            Height = 16,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Untitled",
            Slug = "untitled",
            Description = "Oil on canvas.",
            ImageUrl = "/images/high-res/E969A8CA-3A01-4065-BF4D-3596C65FE4BA.jpeg",
            ThumbnailUrl = "/images/thumbnail/E969A8CA-3A01-4065-BF4D-3596C65FE4BA_.jpg",
            CategorySlug = "paintings",
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Heron in Water",
            Slug = "heron-in-water",
            Description = "Oil on canvas.",
            ImageUrl = "/images/high-res/Heron_in_Water_18_24.jpeg",
            ThumbnailUrl = "/images/thumbnail/Heron_in_Water_18x24_.jpg",
            CategorySlug = "paintings",
            Width = 18,
            Height = 24,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Houses",
            Slug = "houses",
            Description = "Oil on canvas.",
            ImageUrl = "/images/high-res/Houses_20_16.jpeg",
            ThumbnailUrl = "/images/thumbnail/Houses_20_16_.jpg",
            CategorySlug = "paintings",
            Width = 20,
            Height = 16,
            IsAvailable = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Spoonbill",
            Slug = "spoonbill",
            Description = "Oil on canvas.",
            ImageUrl = "/images/high-res/Spoonbill_24_20.jpeg",
            ThumbnailUrl = "/images/thumbnail/Spoonbill_24x20_.jpg",
            CategorySlug = "paintings",
            Width = 24,
            Height = 20,
            IsAvailable = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Waves",
            Slug = "waves",
            Description = "Oil on canvas.",
            ImageUrl = "/images/high-res/Waves_20_24.jpeg",
            ThumbnailUrl = "/images/thumbnail/Waves_20x24_.jpg",
            CategorySlug = "paintings",
            Width = 20,
            Height = 24,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Yellow Boat",
            Slug = "yellow-boat",
            Description = "Oil on canvas.",
            ImageUrl = "/images/high-res/Yellow_Boat_30_24.jpeg",
            ThumbnailUrl = "/images/thumbnail/Yellow_Boat_30_24_.jpg",
            CategorySlug = "paintings",
            Width = 30,
            Height = 24,
            IsAvailable = true,
            IsLandscape = true
        },

        // === Ceramics (8) — no dimensions ===
        new PaintingSeed
        {
            Title = "IMG_0741",
            Slug = "img-0741",
            Description = "Hand-carved ceramic piece using the Sgraffito technique.",
            ImageUrl = "/images/high-res/IMG_0741.jpeg",
            ThumbnailUrl = "/images/thumbnail/IMG_0741_.jpg",
            CategorySlug = "ceramics",
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "IMG_1274",
            Slug = "img-1274",
            Description = "Hand-carved ceramic piece using the Sgraffito technique.",
            ImageUrl = "/images/high-res/IMG_1274.jpeg",
            ThumbnailUrl = "/images/thumbnail/IMG_1274_.jpg",
            CategorySlug = "ceramics",
            IsAvailable = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "IMG_2310",
            Slug = "img-2310",
            Description = "Hand-carved ceramic piece using the Sgraffito technique.",
            ImageUrl = "/images/high-res/IMG_2310.jpeg",
            ThumbnailUrl = "/images/thumbnail/IMG_2310_.jpg",
            CategorySlug = "ceramics",
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "IMG_2555",
            Slug = "img-2555",
            Description = "Hand-carved ceramic piece using the Sgraffito technique.",
            ImageUrl = "/images/high-res/IMG_2555.jpeg",
            ThumbnailUrl = "/images/thumbnail/IMG_2555_.jpg",
            CategorySlug = "ceramics",
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "IMG_2805",
            Slug = "img-2805",
            Description = "Hand-carved ceramic piece using the Sgraffito technique.",
            ImageUrl = "/images/high-res/IMG_2805.jpeg",
            ThumbnailUrl = "/images/thumbnail/IMG_2805_.jpg",
            CategorySlug = "ceramics",
            IsAvailable = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "IMG_3028",
            Slug = "img-3028",
            Description = "Hand-carved ceramic piece using the Sgraffito technique.",
            ImageUrl = "/images/high-res/IMG_3028.jpeg",
            ThumbnailUrl = "/images/thumbnail/IMG_3028_.jpg",
            CategorySlug = "ceramics",
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "IMG_3077",
            Slug = "img-3077",
            Description = "Hand-carved ceramic piece using the Sgraffito technique.",
            ImageUrl = "/images/high-res/IMG_3077.jpeg",
            ThumbnailUrl = "/images/thumbnail/IMG_3077_.jpg",
            CategorySlug = "ceramics",
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "IMG_3080",
            Slug = "img-3080",
            Description = "Hand-carved ceramic piece using the Sgraffito technique.",
            ImageUrl = "/images/high-res/IMG_3080.jpeg",
            ThumbnailUrl = "/images/thumbnail/IMG_3080_.jpg",
            CategorySlug = "ceramics",
            IsAvailable = true,
            IsLandscape = true
        }
    };
}

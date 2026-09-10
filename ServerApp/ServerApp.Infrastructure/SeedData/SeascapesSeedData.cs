namespace ServerApp.Infrastructure.SeedData;

public static class SeascapesSeedData
{
    public static readonly IEnumerable<PaintingSeed> Seascapes = new[]
    {
        new PaintingSeed
        {
            Title = "Cloud Creatures",
            Slug = "cloud-creatures",
            Description = "Framed oil on canvas. This cloud formation was really seen in the morning at the beach.",
            ImageUrl = "/images/high-res/Cloud_Creatures.jpg",
            ThumbnailUrl = "/images/thumbnail/Cloud_Creatures_.jpg",
            CategorySlug = "seascapes",
            Width = 18,
            Height = 22,
            Price = 900,
            IsAvailable = true,
            IsNew = true
        },
        new PaintingSeed
        {
            Title = "Contemplation",
            Slug = "contemplation",
            Description = "Framed oil on canvas. I wondered what he was thinking as he looked out at the ocean.",
            ImageUrl = "/images/high-res/Contemplation.jpg",
            ThumbnailUrl = "/images/thumbnail/Contemplation_.jpg",
            CategorySlug = "seascapes",
            Width = 18,
            Height = 22,
            Price = 700,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Double Roll",
            Slug = "double-roll",
            Description = "Framed oil on canvas. The Atlantic Ocean has waves coming closer together at the beach than the Pacific Ocean.",
            ImageUrl = "/images/high-res/Double_Roll.jpg",
            ThumbnailUrl = "/images/thumbnail/Double_Roll_.jpg",
            CategorySlug = "seascapes",
            Width = 24,
            Height = 18,
            Price = 700,
            IsAvailable = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Even Cloudy Days are Better at the Beach",
            Slug = "even-cloudy-days-are-better-at-the-beach",
            Description = "Framed oil on canvas. Even on a cloudy day there is excitement at seeing the ocean and beach for the first time after many months.",
            ImageUrl = "/images/high-res/Even_Cloudy_Days_are_Better_at_the_Beach.jpg",
            ThumbnailUrl = "/images/thumbnail/Even_Cloudy_Days_are_Better_at_the_Beach_.jpg",
            CategorySlug = "seascapes",
            Width = 23,
            Height = 27,
            Price = 900,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Mangrove Village",
            Slug = "mangrove-village",
            Description = "Oil on gallery wrapped canvas. Mangroves protect coastlines from storm surge and erosion and essential nurseries for marine life, as seen in this painting.",
            ImageUrl = "/images/high-res/Mangrove_Village.jpg",
            ThumbnailUrl = "/images/thumbnail/Mangrove_Village_.jpg",
            CategorySlug = "seascapes",
            Width = 40,
            Height = 30,
            IsAvailable = false,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Morning Glory",
            Slug = "morning-glory",
            Description = "Oil on gallery wrapped canvas. Morning sunrises on the beach near where I live are spectacular.",
            ImageUrl = "/images/high-res/Morning_Glory.jpg",
            ThumbnailUrl = "/images/thumbnail/Morning_Glory_.jpg",
            CategorySlug = "seascapes",
            Width = 30,
            Height = 24,
            Price = 1200,
            IsAvailable = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Pastel Morning",
            Slug = "pastel-morning",
            Description = "Framed oil on canvas. Gentle morning sunrise on the beach.",
            ImageUrl = "/images/high-res/Pastel_Morning.jpg",
            ThumbnailUrl = "/images/thumbnail/Pastel_Morning_.jpg",
            CategorySlug = "seascapes",
            Width = 24,
            Height = 30,
            IsAvailable = false
        },
        new PaintingSeed
        {
            Title = "Rainbow River",
            Slug = "rainbow-river",
            Description = "Framed oil on canvas. Early morning on the river with all the colors.",
            ImageUrl = "/images/high-res/Rainbow_River.jpg",
            ThumbnailUrl = "/images/thumbnail/Rainbow_River_.jpg",
            CategorySlug = "seascapes",
            Width = 31,
            Height = 25,
            Price = 1600,
            IsAvailable = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Tranquility",
            Slug = "tranquility",
            Description = "Framed oil on canvas. The simplicity and tranquility of a rowboat.",
            ImageUrl = "/images/high-res/Tranquility.jpg",
            ThumbnailUrl = "/images/thumbnail/Tranquility_.jpg",
            CategorySlug = "seascapes",
            Width = 20,
            Height = 16,
            IsAvailable = false,
            IsNew = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Sailing Sunset",
            Slug = "sailing-sunset",
            Description = "Framed oil on canvas. Sunset through the sails was caught in the lagoon.",
            ImageUrl = "/images/high-res/Sailing_Sunset.jpg",
            ThumbnailUrl = "/images/thumbnail/Sailing_Sunset.jpg",
            CategorySlug = "seascapes",
            Width = 12,
            Height = 10,
            IsAvailable = false,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Seeing Red",
            Slug = "seeing-red",
            Description = "Framed oil on canvas. An amazing sunset on the water with wading birds.",
            ImageUrl = "/images/high-res/Seeing_Red.jpg",
            ThumbnailUrl = "/images/thumbnail/Seeing_Red.jpg",
            CategorySlug = "seascapes",
            Width = 23,
            Height = 19,
            Price = 750,
            IsAvailable = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Solitude",
            Slug = "solitude",
            Description = "Framed oil on canvas. The rowboat is waiting for a journey to a peaceful shore.",
            ImageUrl = "/images/high-res/Solitude.jpg",
            ThumbnailUrl = "/images/thumbnail/Solitude.jpg",
            CategorySlug = "seascapes",
            Width = 39,
            Height = 27,
            Price = 2500,
            IsAvailable = true,
            IsNew = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Blue Wave",
            Slug = "blue-wave",
            Description = "Framed oil on canvas. The waves can be that blue.",
            ImageUrl = "/images/high-res/Blue_Wave.jpg",
            ThumbnailUrl = "/images/thumbnail/Blue_Wave.jpg",
            CategorySlug = "seascapes",
            Width = 28,
            Height = 24,
            Price = 1500,
            IsAvailable = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Wind & Water",
            Slug = "wind-and-water",
            Description = "Framed oil on canvas. Sailboat color for a client.",
            ImageUrl = "/images/high-res/Wind_and_Water.jpg",
            ThumbnailUrl = "/images/thumbnail/Wind_and_Water_.jpg",
            CategorySlug = "seascapes",
            Width = 40,
            Height = 30,
            IsAvailable = false,
            IsNew = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "To the Light",
            Slug = "to-the-light",
            Description = "Framed oil on canvas. Each year turtles hatch from eggs that are buried in the beach sands in southern Florida. Hatchlings instinctively head toward the bright early morning sunlight at the ocean horizon to avoid predators.",
            ImageUrl = "/images/high-res/To_the_Light.jpg",
            ThumbnailUrl = "/images/thumbnail/To_the_Light_.jpg",
            CategorySlug = "seascapes",
            Width = 12,
            Height = 21,
            Price = 730,
            IsAvailable = true
        }
    };
}
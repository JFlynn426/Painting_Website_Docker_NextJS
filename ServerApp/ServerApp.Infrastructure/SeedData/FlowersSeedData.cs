namespace ServerApp.Infrastructure.SeedData;

public static class FlowersSeedData
{
    public static readonly IEnumerable<PaintingSeed> Flowers = new[]
    {
        new PaintingSeed
        {
            Title = "Bird of Paradise",
            Slug = "bird-of-paradise",
            Description = "Framed oil on canvas. The bird of paradise is native to South Africa where it is called the crane flower. It grows well in South Florida.",
            ImageUrl = "/Flowers-Full/Bird_of_Paradise.jpg",
            ThumbnailUrl = "/Flowers-Thumbnail/Bird_of_Paradise_.jpg",
            CategorySlug = "flowers",
            Width = 46,
            Height = 35,
            Price = 2700,
            IsAvailable = true,
            IsNew = true
        },
        new PaintingSeed
        {
            Title = "Bumblebee & Verbena",
            Slug = "bumblebee-and-verbena",
            Description = "Oil on gallery wrapped canvas. Bumblebee species are declining in Europe, North America, and Asia due to several factors, including land-use and in North America pathogens.",
            ImageUrl = "/Flowers-Full/Bumblebee_Verbena.jpg",
            ThumbnailUrl = "/Flowers-Thumbnail/Bumblebee_Verbena_.jpg",
            CategorySlug = "flowers",
            Width = 8,
            Height = 6,
            IsAvailable = false
        },
        new PaintingSeed
        {
            Title = "Royal Poinciana",
            Slug = "royal-poinciana",
            Description = "Framed oil on canvas. The Royal Poinciana is a tropical tree that blooms in August and loses its leaves during the winter months. Spectacular blossoms for plein air painting.",
            ImageUrl = "/Flowers-Full/Royal_Poinciana.jpg",
            ThumbnailUrl = "/Flowers-Thumbnail/Royal_Poinciana_.jpg",
            CategorySlug = "flowers",
            Width = 16,
            Height = 14,
            Price = 400,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "The Bee & Coneflowers",
            Slug = "the-bee-and-coneflowers",
            Description = "Framed oil on canvas. Our honey bees are valuable to all of us for their pollination of our fruits, vegetables, plants and trees, not to mention the honey that we consume. Coneflowers are native to North America and are resilient, have long bloom time, and are the herb Echinacea known for its medicinal properties (Echinacea).",
            ImageUrl = "/Flowers-Full/Bee_Coneflowers.jpg",
            ThumbnailUrl = "/Flowers-Thumbnail/Bee_Coneflowers_.jpg",
            CategorySlug = "flowers",
            Width = 18,
            Height = 14,
            Price = 600,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Daffodils",
            Slug = "daffodils",
            Description = "Framed oil on canvas. Daffodils are considered a sign of spring and hope.",
            ImageUrl = "/Flowers-Full/Daffodils.jpg",
            ThumbnailUrl = "/Flowers-Thumbnail/Daffodils_.jpg",
            CategorySlug = "flowers",
            Width = 12,
            Height = 16,
            IsAvailable = false
        },
        new PaintingSeed
        {
            Title = "Flowers for Dee",
            Slug = "flowers-for-dee",
            Description = "Framed oil on canvas.",
            ImageUrl = "/Flowers-Full/Flowers_for_Dee.jpg",
            ThumbnailUrl = "/Flowers-Thumbnail/Flowers_for_Dee_.jpg",
            CategorySlug = "flowers",
            Width = 15,
            Height = 12,
            IsAvailable = false
        },
        new PaintingSeed
        {
            Title = "Honeybee & Flowers",
            Slug = "honeybee-and-flowers",
            Description = "Oil on gallery wrapped canvas. Honeybees are declining in the world due to several factors, including land-use and in North America, pathogens.",
            ImageUrl = "/Flowers-Full/Honeybee_Flowers.jpg",
            ThumbnailUrl = "/Flowers-Thumbnail/Honeybee_Flowers_.jpg",
            CategorySlug = "flowers",
            Width = 8,
            Height = 6,
            IsAvailable = false
        },
        new PaintingSeed
        {
            Title = "Safflower Hummer",
            Slug = "safflower-hummer",
            Description = "Framed oil on canvas. For thousands of years safflowers have been grown throughout the world and cultivated for their oilseed. Hummingbirds are found only in the Americas and have amazing abilities.",
            ImageUrl = "/Flowers-Full/Safflower_Hummer.jpg",
            ThumbnailUrl = "/Flowers-Thumbnail/Safflower Hummer_.jpg",
            CategorySlug = "flowers",
            Width = 18,
            Height = 14,
            Price = 600,
            IsAvailable = true,
            IsNew = true
        },
        new PaintingSeed
        {
            Title = "Monarch Life on Milkweed",
            Slug = "monarch-life-on-milkweed",
            Description = "Framed canvas panel. Milkweed is the sole host for monarch butterflies' caterpillars. To prevent monarch butterfly diseases, milkweed needs to experience frost or be cut in the fall in South Florida.",
            ImageUrl = "/Flowers-Full/Monarch_Life_On_Milkweed.jpg",
            ThumbnailUrl = "/Flowers-Thumbnail/Monarch_Life_On_Milkweed_.jpg",
            CategorySlug = "flowers",
            Width = 15,
            Height = 19,
            Price = 600,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Squash Blossoms",
            Slug = "squash-blossoms",
            Description = "Oil on canvas panel. Squash blossoms are edible but only found in the late spring through the summer.",
            ImageUrl = "/Flowers-Full/Squash_blossoms.jpg",
            ThumbnailUrl = "/Flowers-Thumbnail/Squash_blossoms_.jpg",
            CategorySlug = "flowers",
            Width = 12,
            Height = 16,
            Price = 300,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Water Lilies",
            Slug = "water-lilies",
            Description = "Oil on gallery wrapped canvas. Water lilies are aquatic flowering plants that provide a habitat for pond life and help to reduce algae.",
            ImageUrl = "/Flowers-Full/Water_lilies.jpg",
            ThumbnailUrl = "/Flowers-Thumbnail/Water_lilies_.jpg",
            CategorySlug = "flowers",
            Width = 12,
            Height = 24,
            IsAvailable = false
        }
    };
}
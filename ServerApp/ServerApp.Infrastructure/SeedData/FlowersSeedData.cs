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
            ImageUrl = "/images/high-res/Bird_of_Paradise.jpg",
            ThumbnailUrl = "/images/thumbnail/Bird_of_Paradise_.jpg",
            CategorySlug = "flowers",
            Width = 46,
            Height = 35,
            Price = 2700,
            IsAvailable = true,
            IsNew = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Bumblebee & Verbena",
            Slug = "bumblebee-and-verbena",
            Description = "Oil on gallery wrapped canvas. Bumblebee species are declining in Europe, North America, and Asia due to several factors, including land-use and in North America pathogens.",
            ImageUrl = "/images/high-res/Bumblebee_Verbena.jpg",
            ThumbnailUrl = "/images/thumbnail/Bumblebee_Verbena_.jpg",
            CategorySlug = "flowers",
            Width = 8,
            Height = 6,
            IsAvailable = false,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Royal Poinciana",
            Slug = "royal-poinciana",
            Description = "Framed oil on canvas. The Royal Poinciana is a tropical tree that blooms in August and loses its leaves during the winter months. Spectacular blossoms for plein air painting.",
            ImageUrl = "/images/high-res/Royal_Poinciana.jpg",
            ThumbnailUrl = "/images/thumbnail/Royal_Poinciana_.jpg",
            CategorySlug = "flowers",
            Width = 16,
            Height = 14,
            Price = 400,
            IsAvailable = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "The Bee & Coneflowers",
            Slug = "the-bee-and-coneflowers",
            Description = "Framed oil on canvas. Our honey bees are valuable to all of us for their pollination of our fruits, vegetables, plants and trees, not to mention the honey that we consume. Coneflowers are native to North America and are resilient, have long bloom time, and are the herb Echinacea known for its medicinal properties.",
            ImageUrl = "/images/high-res/Bee_Coneflowers.jpg",
            ThumbnailUrl = "/images/thumbnail/Bee_Coneflowers_.jpg",
            CategorySlug = "flowers",
            Width = 18,
            Height = 14,
            Price = 600,
            IsAvailable = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Daffodils",
            Slug = "daffodils",
            Description = "Framed oil on canvas. Daffodils are considered a sign of spring and hope.",
            ImageUrl = "/images/high-res/Daffodils.jpg",
            ThumbnailUrl = "/images/thumbnail/Daffodils_.jpg",
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
            ImageUrl = "/images/high-res/Flowers_for_Dee.jpg",
            ThumbnailUrl = "/images/thumbnail/Flowers_for_Dee_.jpg",
            CategorySlug = "flowers",
            Width = 15,
            Height = 12,
            IsAvailable = false,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Honeybee & Flowers",
            Slug = "honeybee-and-flowers",
            Description = "Oil on gallery wrapped canvas. Honeybees are declining in the world due to several factors, including land-use and in North America, pathogens.",
            ImageUrl = "/images/high-res/Honeybee_Flowers.jpg",
            ThumbnailUrl = "/images/thumbnail/Honeybee_Flowers_.jpg",
            CategorySlug = "flowers",
            Width = 8,
            Height = 6,
            IsAvailable = false,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Safflower Hummer",
            Slug = "safflower-hummer",
            Description = "Framed oil on canvas. For thousands of years safflowers have been grown throughout the world and cultivated for their oilseed. Hummingbirds are found only in the Americas and have amazing abilities.",
            ImageUrl = "/images/high-res/Safflower_Hummer.jpg",
            ThumbnailUrl = "/images/thumbnail/Safflower Hummer_.jpg",
            CategorySlug = "flowers",
            Width = 18,
            Height = 14,
            Price = 600,
            IsAvailable = true,
            IsNew = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Monarch Life on Milkweed",
            Slug = "monarch-life-on-milkweed",
            Description = "Framed canvas panel. Milkweed is the sole host for monarch butterflies' caterpillars. To prevent monarch butterfly diseases, milkweed needs to experience frost or be cut in the fall in South Florida.",
            ImageUrl = "/images/high-res/Monarch_Life_On_Milkweed.jpg",
            ThumbnailUrl = "/images/thumbnail/Monarch_Life_On_Milkweed_.jpg",
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
            ImageUrl = "/images/high-res/Squash_blossoms.jpg",
            ThumbnailUrl = "/images/thumbnail/Squash_blossoms_.jpg",
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
            ImageUrl = "/images/high-res/Water_lilies.jpg",
            ThumbnailUrl = "/images/thumbnail/Water_lilies_.jpg",
            CategorySlug = "flowers",
            Width = 12,
            Height = 24,
            IsAvailable = false
        },
        new PaintingSeed
        {
            Title = "Purple Shamrocks",
            Slug = "purple-shamrocks",
            Description = "Oil on canvas. The purple shamrock can move on its own (photonastic). Its leaves fold up at night and reopen with the morning light.",
            ImageUrl = "/images/high-res/Purple_Flowers.jpg",
            ThumbnailUrl = "/images/thumbnail/Purple_Flowers_.jpg",
            CategorySlug = "flowers",
            Width = 15,
            Height = 19,
            Price = 600,
            IsAvailable = true,
            IsNew = true
        }
    };
}
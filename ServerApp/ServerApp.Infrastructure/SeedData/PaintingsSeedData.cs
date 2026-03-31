namespace ServerApp.Infrastructure.SeedData;

/// <summary>
/// Seed data for the Paintings table.
/// 
/// Table Schema:
/// - Id (uniqueidentifier, PK, ValueGeneratedOnAdd)
/// - Title (nvarchar(200), Required)
/// - Slug (nvarchar(200), Required, Unique Index)
/// - Description (nvarchar(max), Optional)
/// - ImageUrl (nvarchar(500), Required)
/// - ThumbnailUrl (nvarchar(500), Optional)
/// - CategorySlug (nvarchar(100), Required, Index)
/// - Width (decimal(18,2), Optional)
/// - Height (decimal(18,2), Optional)
/// - Depth (decimal(18,2), Optional)
/// - Year (int, Optional)
/// - Price (decimal(18,2), Optional)
/// - IsAvailable (bit, Required)
/// - CategoryId (uniqueidentifier, FK, Index)
/// 
/// This data matches the client-side models in clientapp/src/app/models/paintings.ts
/// to ensure consistency between frontend and backend data.
/// </summary>
public static class PaintingsSeedData
{
    public static readonly List<PaintingSeed> Paintings = new()
    {
        // Landscapes and Cityscapes (18 paintings)
        new PaintingSeed { Title = "Aspens", Slug = "aspens", Description = "A serene autumn scene featuring golden aspen trees against a mountain backdrop.", ImageUrl = "/Landscapes and Cityscapes/Aspens.jpg", CategorySlug = "landscapes-and-cityscapes", Width = 24, Height = 36, Year = 2023, Price = 1200, IsAvailable = true },
        new PaintingSeed { Title = "Bahia Honda", Slug = "bahia-honda", Description = "A tropical paradise capturing the essence of the Florida Keys.", ImageUrl = "/Landscapes and Cityscapes/Bahia Honda .jpg", CategorySlug = "landscapes-and-cityscapes", Width = 30, Height = 40, Year = 2022, Price = 1500, IsAvailable = true },
        new PaintingSeed { Title = "Baked Goods", Slug = "baked-goods", Description = "A charming street scene featuring a local bakery.", ImageUrl = "/Landscapes and Cityscapes/BakedGoods.JPG", CategorySlug = "landscapes-and-cityscapes", Width = 18, Height = 24, Year = 2023, Price = 800, IsAvailable = false },
        new PaintingSeed { Title = "Banyans", Slug = "banyans", Description = "Majestic banyan trees creating a natural cathedral.", ImageUrl = "/Landscapes and Cityscapes/Banyans.jpg", CategorySlug = "landscapes-and-cityscapes", Width = 36, Height = 48, Year = 2022, Price = 2000, IsAvailable = true },
        new PaintingSeed { Title = "Brady & Duke", Slug = "brady-and-duke", Description = "A heartwarming portrait of two beloved companions.", ImageUrl = "/Landscapes and Cityscapes/Brady & Duke.jpg", CategorySlug = "landscapes-and-cityscapes", Width = 20, Height = 24, Year = 2023, Price = 950, IsAvailable = true },
        new PaintingSeed { Title = "Bridge World", Slug = "bridge-world", Description = "An architectural study of bridges connecting worlds.", ImageUrl = "/Landscapes and Cityscapes/Bridge world.jpg", CategorySlug = "landscapes-and-cityscapes", Width = 24, Height = 30, Year = 2021, Price = 1100, IsAvailable = true },
        new PaintingSeed { Title = "Castelvetore", Slug = "castelvetore", Description = "A picturesque Italian village bathed in warm sunlight.", ImageUrl = "/Landscapes and Cityscapes/Castelvetore .JPG", CategorySlug = "landscapes-and-cityscapes", Width = 20, Height = 28, Year = 2022, Price = 1300, IsAvailable = true },
        new PaintingSeed { Title = "Cobblestone Way", Slug = "cobblestone-way", Description = "An old-world charm captured in cobblestone streets.", ImageUrl = "/Landscapes and Cityscapes/Cobblestone Way.JPG", CategorySlug = "landscapes-and-cityscapes", Width = 16, Height = 20, Year = 2023, Price = 750, IsAvailable = false },
        new PaintingSeed { Title = "Forest Mystery", Slug = "forest-mystery", Description = "Enigmatic forest scene with dappled light filtering through trees.", ImageUrl = "/Landscapes and Cityscapes/Forest Mystery.jpg", CategorySlug = "landscapes-and-cityscapes", Width = 30, Height = 36, Year = 2022, Price = 1600, IsAvailable = true },
        new PaintingSeed { Title = "Hydrangea Time", Slug = "hydrangea-time", Description = "Lush hydrangeas in full bloom creating a garden paradise.", ImageUrl = "/Landscapes and Cityscapes/Hydrangea Time.jpeg", CategorySlug = "landscapes-and-cityscapes", Width = 24, Height = 24, Year = 2023, Price = 900, IsAvailable = true },
        new PaintingSeed { Title = "Italian Church", Slug = "italian-church", Description = "A historic church standing as a testament to faith and artistry.", ImageUrl = "/Landscapes and Cityscapes/Italian Church.jpg", CategorySlug = "landscapes-and-cityscapes", Width = 28, Height = 36, Year = 2021, Price = 1400, IsAvailable = true },
        new PaintingSeed { Title = "Mine", Slug = "mine", Description = "An industrial landscape exploring the beauty of working environments.", ImageUrl = "/Landscapes and Cityscapes/Mine (1).jpg", CategorySlug = "landscapes-and-cityscapes", Width = 32, Height = 40, Year = 2022, Price = 1700, IsAvailable = false },
        new PaintingSeed { Title = "New Inhabitants", Slug = "new-inhabitants", Description = "Nature reclaiming space with new life emerging.", ImageUrl = "/Landscapes and Cityscapes/New Inhabitants.jpeg", CategorySlug = "landscapes-and-cityscapes", Width = 20, Height = 24, Year = 2023, Price = 850, IsAvailable = true },
        new PaintingSeed { Title = "Sedona", Slug = "sedona", Description = "The iconic red rocks of Sedona in golden hour light.", ImageUrl = "/Landscapes and Cityscapes/Sedona.JPG", CategorySlug = "landscapes-and-cityscapes", Width = 30, Height = 40, Year = 2021, Price = 1800, IsAvailable = true },
        new PaintingSeed { Title = "The Path", Slug = "the-path", Description = "A winding path inviting viewers into a contemplative journey.", ImageUrl = "/Landscapes and Cityscapes/The Path.jpg", CategorySlug = "landscapes-and-cityscapes", Width = 24, Height = 30, Year = 2023, Price = 1100, IsAvailable = true },
        new PaintingSeed { Title = "The Pond", Slug = "the-pond", Description = "A tranquil pond reflecting the surrounding nature.", ImageUrl = "/Landscapes and Cityscapes/The Pond.jpg", CategorySlug = "landscapes-and-cityscapes", Width = 20, Height = 28, Year = 2022, Price = 950, IsAvailable = false },
        new PaintingSeed { Title = "Window Jigsaw", Slug = "window-jigsaw", Description = "An abstract view through windows creating a puzzle of perspectives.", ImageUrl = "/Landscapes and Cityscapes/Window Jigsaw.jpg", CategorySlug = "landscapes-and-cityscapes", Width = 18, Height = 24, Year = 2023, Price = 800, IsAvailable = true },

        // Seascapes (17 paintings)
        new PaintingSeed { Title = "Cloud Creatures", Slug = "cloud-creatures", Description = "Whimsical cloud formations dancing across the sky above the ocean.", ImageUrl = "/Seascapes/Cloud Creatures2.jpg", CategorySlug = "seascapes", Width = 30, Height = 40, Year = 2023, Price = 1600, IsAvailable = true },
        new PaintingSeed { Title = "Contemplation", Slug = "contemplation", Description = "A moment of quiet reflection by the waters edge.", ImageUrl = "/Seascapes/Contemplation.jpeg", CategorySlug = "seascapes", Width = 24, Height = 32, Year = 2022, Price = 1200, IsAvailable = true },
        new PaintingSeed { Title = "Double Roll", Slug = "double-roll", Description = "Two waves rolling in perfect harmony toward the shore.", ImageUrl = "/Seascapes/Double Roll.jpg", CategorySlug = "seascapes", Width = 36, Height = 48, Year = 2021, Price = 2200, IsAvailable = false },
        new PaintingSeed { Title = "Even Cloudy Days are Better at the Beach", Slug = "even-cloudy-days-are-better-at-the-beach", Description = "Finding beauty in overcast coastal scenes.", ImageUrl = "/Seascapes/Even Cloudy Days are Better at the Beach.jpeg", CategorySlug = "seascapes", Width = 24, Height = 30, Year = 2023, Price = 1100, IsAvailable = true },
        new PaintingSeed { Title = "Mangrove Village", Slug = "mangrove-village", Description = "A coastal village nestled among mangrove trees.", ImageUrl = "/Seascapes/Mangrove Village.jpg", CategorySlug = "seascapes", Width = 28, Height = 36, Year = 2022, Price = 1400, IsAvailable = true },
        new PaintingSeed { Title = "Morning Glory", Slug = "morning-glory", Description = "The first light of dawn breaking over the ocean.", ImageUrl = "/Seascapes/Morning Glory.jpg", CategorySlug = "seascapes", Width = 30, Height = 40, Year = 2023, Price = 1700, IsAvailable = true },
        new PaintingSeed { Title = "Pastel Morning", Slug = "pastel-morning", Description = "Soft pastel hues painting the early morning sky.", ImageUrl = "/Seascapes/Pastel Morning.JPG", CategorySlug = "seascapes", Width = 24, Height = 32, Year = 2022, Price = 1250, IsAvailable = true },
        new PaintingSeed { Title = "Rainbow River", Slug = "rainbow-river-seascape", Description = "Where the river meets the sea in a colorful embrace.", ImageUrl = "/Seascapes/Rainbow River.jpg", CategorySlug = "seascapes", Width = 26, Height = 34, Year = 2021, Price = 1350, IsAvailable = false },
        new PaintingSeed { Title = "Rowboat Waiting", Slug = "rowboat-waiting", Description = "A solitary rowboat resting on calm waters.", ImageUrl = "/Seascapes/Rowboat Waiting.jpg", CategorySlug = "seascapes", Width = 20, Height = 28, Year = 2023, Price = 950, IsAvailable = true },
        new PaintingSeed { Title = "Sailboat", Slug = "sailboat", Description = "A graceful sailboat cutting through the waves.", ImageUrl = "/Seascapes/Sailboat.jpg", CategorySlug = "seascapes", Width = 24, Height = 30, Year = 2022, Price = 1150, IsAvailable = true },
        new PaintingSeed { Title = "Sailing Sunset", Slug = "sailing-sunset", Description = "Silhouetted sails against a dramatic sunset.", ImageUrl = "/Seascapes/Sailing Sunset.jpg", CategorySlug = "seascapes", Width = 32, Height = 40, Year = 2021, Price = 1800, IsAvailable = true },
        new PaintingSeed { Title = "Seeing Red", Slug = "seeing-red", Description = "Bold red tones capturing the intensity of coastal light.", ImageUrl = "/Seascapes/Seeing Red.jpg", CategorySlug = "seascapes", Width = 24, Height = 32, Year = 2023, Price = 1200, IsAvailable = true },
        new PaintingSeed { Title = "Shore Scene", Slug = "shore-scene", Description = "A peaceful shoreline with gentle waves lapping at the sand.", ImageUrl = "/Seascapes/Shore scene.JPG", CategorySlug = "seascapes", Width = 20, Height = 24, Year = 2022, Price = 900, IsAvailable = false },
        new PaintingSeed { Title = "Solitude", Slug = "solitude", Description = "The peaceful isolation of being alone with the ocean.", ImageUrl = "/Seascapes/Solitude.jpg", CategorySlug = "seascapes", Width = 28, Height = 36, Year = 2023, Price = 1450, IsAvailable = true },
        new PaintingSeed { Title = "Sunrise Drama", Slug = "sunrise-drama", Description = "Dramatic clouds and light at the moments of sunrise.", ImageUrl = "/Seascapes/SunriseDrama.JPG", CategorySlug = "seascapes", Width = 30, Height = 40, Year = 2022, Price = 1650, IsAvailable = true },
        new PaintingSeed { Title = "Wave Blue", Slug = "wave-blue", Description = "The deep blue of ocean waves in motion.", ImageUrl = "/Seascapes/Wave Blue.jpg", CategorySlug = "seascapes", Width = 24, Height = 30, Year = 2021, Price = 1100, IsAvailable = true },
        new PaintingSeed { Title = "Wind and Water", Slug = "wind-and-water", Description = "The dynamic interplay between wind and water.", ImageUrl = "/Seascapes/Wind and Water1.JPG", CategorySlug = "seascapes", Width = 26, Height = 34, Year = 2023, Price = 1300, IsAvailable = true },

        // Animals & People (25 paintings)
        new PaintingSeed { Title = "Abby's Horse", Slug = "abbys-horse", Description = "A majestic portrait of Abby's beloved horse.", ImageUrl = "/Animals/Abby's Horse.jpg", CategorySlug = "animals-and-people", Width = 30, Height = 40, Year = 2023, Price = 1800, IsAvailable = true },
        new PaintingSeed { Title = "Angelfish & Purple Fan", Slug = "angelfish-and-purple-fan", Description = "Vibrant underwater scene featuring colorful angelfish.", ImageUrl = "/Animals/Angelfish  & Purple Fan.jpg", CategorySlug = "animals-and-people", Width = 24, Height = 32, Year = 2022, Price = 1400, IsAvailable = true },
        new PaintingSeed { Title = "Cruising", Slug = "cruising", Description = "Marine life cruising through the ocean depths.", ImageUrl = "/Animals/Cruising.JPG", CategorySlug = "animals-and-people", Width = 20, Height = 28, Year = 2023, Price = 1100, IsAvailable = false },
        new PaintingSeed { Title = "Dyptych Big Yellow", Slug = "dyptych-big-yellow", Description = "A striking dyptych featuring bold yellow tones.", ImageUrl = "/Animals/Dyptych Big Yellow.jpg", CategorySlug = "animals-and-people", Width = 36, Height = 48, Year = 2021, Price = 2400, IsAvailable = true },
        new PaintingSeed { Title = "Dyptych Ocean Butterflies", Slug = "dyptych-ocean-butterflies", Description = "Two panels capturing the grace of ocean butterflies.", ImageUrl = "/Animals/Dyptych Ocean butterflies.jpg", CategorySlug = "animals-and-people", Width = 40, Height = 30, Year = 2022, Price = 2200, IsAvailable = true },
        new PaintingSeed { Title = "Ever Vigilant", Slug = "ever-vigilant", Description = "A watchful creature ever alert to its surroundings.", ImageUrl = "/Animals/Ever Vigilant.jpg", CategorySlug = "animals-and-people", Width = 24, Height = 30, Year = 2023, Price = 1250, IsAvailable = true },
        new PaintingSeed { Title = "Fairy Wrens", Slug = "fairy-wrens", Description = "Delicate fairy wrens in their natural habitat.", ImageUrl = "/Animals/Fairy Wrens.jpg", CategorySlug = "animals-and-people", Width = 18, Height = 24, Year = 2022, Price = 850, IsAvailable = true },
        new PaintingSeed { Title = "Green Turtle", Slug = "green-turtle", Description = "A serene green turtle gliding through turquoise waters.", ImageUrl = "/Animals/Green Turtle.JPG", CategorySlug = "animals-and-people", Width = 28, Height = 36, Year = 2021, Price = 1600, IsAvailable = false },
        new PaintingSeed { Title = "Hiding", Slug = "hiding", Description = "A playful creature caught in a moment of hiding.", ImageUrl = "/Animals/Hiding.JPG", CategorySlug = "animals-and-people", Width = 20, Height = 24, Year = 2023, Price = 950, IsAvailable = true },
        new PaintingSeed { Title = "Leatherback", Slug = "leatherback", Description = "The magnificent leatherback turtle in its element.", ImageUrl = "/Animals/Leatherback .jpg", CategorySlug = "animals-and-people", Width = 32, Height = 40, Year = 2022, Price = 1750, IsAvailable = true },
        new PaintingSeed { Title = "Manatees", Slug = "manatees", Description = "Gentle manatees floating in peaceful waters.", ImageUrl = "/Animals/Manatees.jpg", CategorySlug = "animals-and-people", Width = 24, Height = 32, Year = 2023, Price = 1300, IsAvailable = true },
        new PaintingSeed { Title = "Meadow", Slug = "meadow", Description = "Wild animals grazing in a sunlit meadow.", ImageUrl = "/Animals/Meadow.JPG", CategorySlug = "animals-and-people", Width = 30, Height = 40, Year = 2022, Price = 1550, IsAvailable = true },
        new PaintingSeed { Title = "My Green Visitor", Slug = "my-green-visitor", Description = "An unexpected green visitor bringing life to the scene.", ImageUrl = "/Animals/My Green Visitor.jpg", CategorySlug = "animals-and-people", Width = 20, Height = 28, Year = 2021, Price = 1050, IsAvailable = false },
        new PaintingSeed { Title = "Octopus Purple", Slug = "octopus-purple", Description = "A mesmerizing purple octopus in all its tentacled glory.", ImageUrl = "/Animals/OctopusPurple.jpg", CategorySlug = "animals-and-people", Width = 24, Height = 24, Year = 2023, Price = 1150, IsAvailable = true },
        new PaintingSeed { Title = "Painted Bunting", Slug = "painted-bunting", Description = "The brilliantly colored painted bunting in flight.", ImageUrl = "/Animals/Painted Bunting .jpg", CategorySlug = "animals-and-people", Width = 16, Height = 20, Year = 2022, Price = 750, IsAvailable = true },
        new PaintingSeed { Title = "Peekaboo", Slug = "peekaboo", Description = "A playful peekaboo moment with a curious creature.", ImageUrl = "/Animals/Peekaboo.jpg", CategorySlug = "animals-and-people", Width = 18, Height = 24, Year = 2023, Price = 850, IsAvailable = true },
        new PaintingSeed { Title = "Sam", Slug = "sam", Description = "A portrait of Sam, a beloved companion.", ImageUrl = "/Animals/Sam.JPG", CategorySlug = "animals-and-people", Width = 20, Height = 24, Year = 2022, Price = 900, IsAvailable = false },
        new PaintingSeed { Title = "Snail Kite", Slug = "snail-kite", Description = "The elegant snail kite in its natural wetland habitat.", ImageUrl = "/Animals/Snail Kite .jpg", CategorySlug = "animals-and-people", Width = 24, Height = 30, Year = 2021, Price = 1200, IsAvailable = true },
        new PaintingSeed { Title = "Snowy Owl", Slug = "snowy-owl", Description = "A majestic snowy owl with piercing gaze.", ImageUrl = "/Animals/SnowyOwl.jpg", CategorySlug = "animals-and-people", Width = 28, Height = 36, Year = 2023, Price = 1650, IsAvailable = true },
        new PaintingSeed { Title = "Spiny Lobster", Slug = "spiny-lobster", Description = "A colorful spiny lobster in its underwater world.", ImageUrl = "/Animals/Spiny Lobster.jpg", CategorySlug = "animals-and-people", Width = 20, Height = 28, Year = 2022, Price = 1050, IsAvailable = true },
        new PaintingSeed { Title = "Tigers", Slug = "tigers", Description = "The power and grace of tigers captured in paint.", ImageUrl = "/Animals/Tigers.jpg", CategorySlug = "animals-and-people", Width = 36, Height = 48, Year = 2021, Price = 2500, IsAvailable = true },
        new PaintingSeed { Title = "To the Light", Slug = "to-the-light", Description = "Creatures drawn to the light in a dramatic scene.", ImageUrl = "/Animals/To the Light.jpg", CategorySlug = "animals-and-people", Width = 24, Height = 32, Year = 2023, Price = 1350, IsAvailable = false },
        new PaintingSeed { Title = "What's for Dinner", Slug = "whats-for-dinner", Description = "A humorous take on the food chain in nature.", ImageUrl = "/Animals/What's for dinner.jpg", CategorySlug = "animals-and-people", Width = 22, Height = 28, Year = 2022, Price = 1100, IsAvailable = true },
        new PaintingSeed { Title = "Wild White Majesty", Slug = "wild-white-majesty", Description = "The wild white majesty of nature's creatures.", ImageUrl = "/Animals/Wild White Majesty.jpg", CategorySlug = "animals-and-people", Width = 30, Height = 40, Year = 2021, Price = 1800, IsAvailable = true },
        new PaintingSeed { Title = "ZsaZsa", Slug = "zsazsa", Description = "A charming portrait of ZsaZsa with personality.", ImageUrl = "/Animals/ZsaZsa.jpeg", CategorySlug = "animals-and-people", Width = 18, Height = 24, Year = 2023, Price = 850, IsAvailable = true },

        // Flowers (12 paintings)
        new PaintingSeed { Title = "Bird of Paradise", Slug = "bird-of-paradise", Description = "The exotic beauty of the bird of paradise flower.", ImageUrl = "/Flowers/Bird of Paradise.jpg", CategorySlug = "flowers", Width = 24, Height = 30, Year = 2023, Price = 1200, IsAvailable = true },
        new PaintingSeed { Title = "Bumblebee", Slug = "bumblebee", Description = "A busy bumblebee collecting nectar from vibrant blooms.", ImageUrl = "/Flowers/Bumblebee.jpg", CategorySlug = "flowers", Width = 20, Height = 24, Year = 2022, Price = 950, IsAvailable = true },
        new PaintingSeed { Title = "Coneflowers", Slug = "coneflowers", Description = "Rustic coneflowers swaying in a summer breeze.", ImageUrl = "/Flowers/Coneflowers.jpg", CategorySlug = "flowers", Width = 24, Height = 32, Year = 2023, Price = 1150, IsAvailable = false },
        new PaintingSeed { Title = "Daffodils", Slug = "daffodils", Description = "Cheerful daffodils heralding the arrival of spring.", ImageUrl = "/Flowers/Daffodils.jpg", CategorySlug = "flowers", Width = 20, Height = 28, Year = 2022, Price = 1000, IsAvailable = true },
        new PaintingSeed { Title = "Flowers for Dee", Slug = "flowers-for-dee", Description = "A special floral tribute created for Dee.", ImageUrl = "/Flowers/Flowers for Dee.jpg", CategorySlug = "flowers", Width = 24, Height = 30, Year = 2021, Price = 1250, IsAvailable = true },
        new PaintingSeed { Title = "Honey Bee", Slug = "honey-bee", Description = "A honey bee at work among colorful blossoms.", ImageUrl = "/Flowers/Honey bee.jpeg", CategorySlug = "flowers", Width = 18, Height = 24, Year = 2023, Price = 850, IsAvailable = true },
        new PaintingSeed { Title = "Milkweed Hummingbird", Slug = "milkweed-hummingbird", Description = "A hummingbird feeding on milkweed nectar.", ImageUrl = "/Flowers/MilkweedHummingbird.jpg", CategorySlug = "flowers", Width = 22, Height = 28, Year = 2022, Price = 1100, IsAvailable = true },
        new PaintingSeed { Title = "Monarch Beginnings", Slug = "monarch-beginnings", Description = "The beginning of a monarch butterfly's journey.", ImageUrl = "/Flowers/Monarch Beginnings.JPG", CategorySlug = "flowers", Width = 20, Height = 24, Year = 2023, Price = 950, IsAvailable = false },
        new PaintingSeed { Title = "Royal Poinciana", Slug = "royal-poinciana", Description = "The regal royal poinciana in full tropical bloom.", ImageUrl = "/Flowers/Royal Poinciana.jpg", CategorySlug = "flowers", Width = 28, Height = 36, Year = 2021, Price = 1500, IsAvailable = true },
        new PaintingSeed { Title = "Squash Blossoms", Slug = "squash-blossoms", Description = "Golden squash blossoms in a garden setting.", ImageUrl = "/Flowers/Squash blossoms1.JPG", CategorySlug = "flowers", Width = 20, Height = 24, Year = 2022, Price = 900, IsAvailable = true },
        new PaintingSeed { Title = "The Bee", Slug = "the-bee", Description = "A close-up study of a bee in its natural habitat.", ImageUrl = "/Flowers/The Bee.jpg", CategorySlug = "flowers", Width = 16, Height = 20, Year = 2023, Price = 750, IsAvailable = true },
        new PaintingSeed { Title = "Water Lilies", Slug = "water-lilies", Description = "Peaceful water lilies floating on a tranquil pond.", ImageUrl = "/Flowers/Water lilies.jpg", CategorySlug = "flowers", Width = 24, Height = 32, Year = 2022, Price = 1200, IsAvailable = true }
    };
}

/// <summary>
/// Represents seed data for a painting.
/// Matches the PaintingDto structure.
/// </summary>
public class PaintingSeed
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
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
}
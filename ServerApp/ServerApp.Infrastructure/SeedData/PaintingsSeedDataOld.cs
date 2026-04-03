/* namespace ServerApp.Infrastructure.SeedData;

public static class PaintingsSeedData
{
    public static readonly IEnumerable<PaintingSeed> Seascapes = new[]
    {
        new PaintingSeed
        {
            Title = "Cloud Creatures2",
            Slug = "cloud-creatures2",
            Description = "A dramatic seascape featuring cloud formations",
            ImageUrl = "/Seascapes/Cloud_Creatures2.jpg",
            CategorySlug = "seascapes",
            Width = 4800,
            Height = 6000,
            Year = 2023,
            Price = 1500,
            IsAvailable = true,
            IsNew = true
        },
        new PaintingSeed
        {
            Title = "Contemplation",
            Slug = "contemplation",
            Description = "A peaceful moment by the water",
            ImageUrl = "/Seascapes/Contemplation.jpeg",
            CategorySlug = "seascapes",
            Width = 3000,
            Height = 4000,
            Year = 2023,
            Price = 1200,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Double Roll",
            Slug = "double-roll",
            Description = "Rolling waves captured in motion",
            ImageUrl = "/Seascapes/Double_Roll.jpg",
            CategorySlug = "seascapes",
            Width = 4032,
            Height = 3024,
            Year = 2023,
            Price = 1800,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Even Cloudy Days are Better at the Beach",
            Slug = "even-cloudy-days-are-better-at-the-beach",
            Description = "Finding beauty in overcast coastal scenes",
            ImageUrl = "/Seascapes/Even_Cloudy_Days_are_Better_at_the_Beach.jpeg",
            CategorySlug = "seascapes",
            Width = 3000,
            Height = 4000,
            Year = 2023,
            Price = 1400,
            IsAvailable = true,
            IsNew = true
        },
        new PaintingSeed
        {
            Title = "Mangrove Village",
            Slug = "mangrove-village",
            Description = "A coastal village nestled among mangroves",
            ImageUrl = "/Seascapes/Mangrove_Village.jpg",
            CategorySlug = "seascapes",
            Width = 4000,
            Height = 3000,
            Year = 2023,
            Price = 1600,
            IsAvailable = true,
            IsNew = true
        },
        new PaintingSeed
        {
            Title = "Morning Glory",
            Slug = "morning-glory",
            Description = "Early morning light on the water",
            ImageUrl = "/Seascapes/Morning_Glory.jpg",
            CategorySlug = "seascapes",
            Width = 4000,
            Height = 3000,
            Year = 2023,
            Price = 1300,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Pastel Morning",
            Slug = "pastel-morning",
            Description = "Soft pastel colors of dawn",
            ImageUrl = "/Seascapes/Pastel_Morning.JPG",
            CategorySlug = "seascapes",
            Width = 3000,
            Height = 4000,
            Year = 2023,
            Price = 1100,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Rainbow River",
            Slug = "rainbow-river",
            Description = "Vibrant colors reflecting on the river",
            ImageUrl = "/Seascapes/Rainbow_River.jpg",
            CategorySlug = "seascapes",
            Width = 4000,
            Height = 3000,
            Year = 2023,
            Price = 1700,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Rowboat Waiting",
            Slug = "rowboat-waiting",
            Description = "A solitary rowboat at rest",
            ImageUrl = "/Seascapes/Rowboat_Waiting.jpg",
            CategorySlug = "seascapes",
            Width = 4000,
            Height = 3000,
            Year = 2023,
            Price = 1200,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Sailboat",
            Slug = "sailboat",
            Description = "A sailboat gliding across the water",
            ImageUrl = "/Seascapes/Sailboat.jpg",
            CategorySlug = "seascapes",
            Width = 3000,
            Height = 4000,
            Year = 2023,
            Price = 1500,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Sailing Sunset",
            Slug = "sailing-sunset",
            Description = "Sunset colors on a sailing adventure",
            ImageUrl = "/Seascapes/Sailing_Sunset.jpg",
            CategorySlug = "seascapes",
            Width = 4000,
            Height = 3000,
            Year = 2023,
            Price = 1900,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Seeing Red",
            Slug = "seeing-red",
            Description = "Bold red tones in a seascape",
            ImageUrl = "/Seascapes/Seeing_Red.jpg",
            CategorySlug = "seascapes",
            Width = 4000,
            Height = 3000,
            Year = 2023,
            Price = 1400,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Shore scene",
            Slug = "shore-scene",
            Description = "A tranquil shore scene",
            ImageUrl = "/Seascapes/Shore_scene.JPG",
            CategorySlug = "seascapes",
            Width = 3000,
            Height = 4000,
            Year = 2023,
            Price = 1300,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Solitude",
            Slug = "solitude",
            Description = "The peace of being alone by the sea",
            ImageUrl = "/Seascapes/Solitude.jpg",
            CategorySlug = "seascapes",
            Width = 4000,
            Height = 3000,
            Year = 2023,
            Price = 1600,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "SunriseDrama",
            Slug = "sunrisedrama",
            Description = "Dramatic sunrise over the ocean",
            ImageUrl = "/Seascapes/SunriseDrama.JPG",
            CategorySlug = "seascapes",
            Width = 4000,
            Height = 3000,
            Year = 2023,
            Price = 1800,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Wave Blue",
            Slug = "wave-blue",
            Description = "Blue waves in motion",
            ImageUrl = "/Seascapes/Wave_Blue.jpg",
            CategorySlug = "seascapes",
            Width = 4000,
            Height = 3000,
            Year = 2023,
            Price = 1500,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Wind and Water1",
            Slug = "wind-and-water1",
            Description = "The interplay of wind and water",
            ImageUrl = "/Seascapes/Wind_and_Water1.JPG",
            CategorySlug = "seascapes",
            Width = 4000,
            Height = 3000,
            Year = 2023,
            Price = 1700,
            IsAvailable = true,
            IsNew = true
        }
    };

    public static readonly IEnumerable<PaintingSeed> Animals = new[]
    {
        new PaintingSeed
        {
            Title = "Abby's Horse",
            Slug = "abbys-horse",
            Description = "A portrait of Abby's beloved horse",
            ImageUrl = "/Animals/Abbys_Horse.jpg",
            CategorySlug = "animals-and-people",
            Width = 4200,
            Height = 5400,
            Year = 2023,
            Price = 2000,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Angelfish & Purple Fan",
            Slug = "angelfish-purple-fan",
            Description = "Colorful angelfish near a purple fan coral",
            ImageUrl = "/Animals/Angelfish__&_Purple_Fan.jpg",
            CategorySlug = "animals-and-people",
            Width = 6000,
            Height = 4800,
            Year = 2023,
            Price = 1800,
            IsAvailable = true,
            IsNew = true
        },
        new PaintingSeed
        {
            Title = "Cruising",
            Slug = "cruising",
            Description = "A cruising scene",
            ImageUrl = "/Animals/Cruising.JPG",
            CategorySlug = "animals-and-people",
            Width = 3264,
            Height = 2448,
            Year = 2023,
            Price = 1500,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Dyptych Big Yellow",
            Slug = "dyptych-big-yellow",
            Description = "A yellow diptych composition",
            ImageUrl = "/Animals/Dyptych_Big_Yellow.jpg",
            CategorySlug = "animals-and-people",
            Width = 6026,
            Height = 6003,
            Year = 2023,
            Price = 2200,
            IsAvailable = true,
            IsNew = true
        },
        new PaintingSeed
        {
            Title = "Dyptych Ocean Butterflies",
            Slug = "dyptych-ocean-butterflies",
            Description = "Ocean butterflies diptych",
            ImageUrl = "/Animals/Dyptych_Ocean_butterflies.jpg",
            CategorySlug = "animals-and-people",
            Width = 2800,
            Height = 2800,
            Year = 2023,
            Price = 1900,
            IsAvailable = true,
            IsNew = true
        },
        new PaintingSeed
        {
            Title = "Ever Vigilant",
            Slug = "ever-vigilant",
            Description = "Always watching",
            ImageUrl = "/Animals/Ever_Vigilant.jpg",
            CategorySlug = "animals-and-people",
            Width = 1639,
            Height = 2183,
            Year = 2023,
            Price = 1600,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Fairy Wrens",
            Slug = "fairy-wrens",
            Description = "Colorful fairy wrens",
            ImageUrl = "/Animals/Fairy_Wrens.jpg",
            CategorySlug = "animals-and-people",
            Width = 3600,
            Height = 1800,
            Year = 2023,
            Price = 1400,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Green Turtle",
            Slug = "green-turtle",
            Description = "A green sea turtle",
            ImageUrl = "/Animals/Green_Turtle.JPG",
            CategorySlug = "animals-and-people",
            Width = 3264,
            Height = 2448,
            Year = 2023,
            Price = 1700,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Hiding",
            Slug = "hiding",
            Description = "Something hiding",
            ImageUrl = "/Animals/Hiding.JPG",
            CategorySlug = "animals-and-people",
            Width = 1512,
            Height = 2016,
            Year = 2023,
            Price = 1300,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Leatherback",
            Slug = "leatherback",
            Description = "A leatherback turtle",
            ImageUrl = "/Animals/Leatherback_.jpg",
            CategorySlug = "animals-and-people",
            Width = 1549,
            Height = 1290,
            Year = 2023,
            Price = 1500,
            IsAvailable = true,
            IsNew = true
        },
        new PaintingSeed
        {
            Title = "Manatees",
            Slug = "manatees",
            Description = "Gentle manatees",
            ImageUrl = "/Animals/Manatees.jpg",
            CategorySlug = "animals-and-people",
            Width = 6000,
            Height = 4930,
            Year = 2023,
            Price = 1800,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Meadow",
            Slug = "meadow",
            Description = "A peaceful meadow",
            ImageUrl = "/Animals/Meadow.JPG",
            CategorySlug = "animals-and-people",
            Width = 4032,
            Height = 3024,
            Year = 2023,
            Price = 1600,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "My Green Visitor",
            Slug = "my-green-visitor",
            Description = "A green visitor",
            ImageUrl = "/Animals/My_Green_Visitor.jpg",
            CategorySlug = "animals-and-people",
            Width = 2100,
            Height = 1576,
            Year = 2023,
            Price = 1400,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Octopus Purple",
            Slug = "octopus-purple",
            Description = "A purple octopus",
            ImageUrl = "/Animals/OctopusPurple.jpg",
            CategorySlug = "animals-and-people",
            Width = 3900,
            Height = 5850,
            Year = 2023,
            Price = 1900,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Painted Bunting",
            Slug = "painted-bunting",
            Description = "A colorful painted bunting",
            ImageUrl = "/Animals/Painted_Bunting_.jpg",
            CategorySlug = "animals-and-people",
            Width = 1000,
            Height = 1333,
            Year = 2023,
            Price = 1200,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Peekaboo",
            Slug = "peekaboo",
            Description = "Playing peekaboo",
            ImageUrl = "/Animals/Peekaboo.jpg",
            CategorySlug = "animals-and-people",
            Width = 6000,
            Height = 4800,
            Year = 2023,
            Price = 1700,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Pink Floy",
            Slug = "pink-floy",
            Description = "Pink floy",
            ImageUrl = "/Animals/Pink_Floy.tif",
            CategorySlug = "animals-and-people",
            Width = 1000,
            Height = 750,
            Year = 2023,
            Price = 1500,
            IsAvailable = true,
            IsNew = true
        },
        new PaintingSeed
        {
            Title = "Sam",
            Slug = "sam",
            Description = "Portrait of Sam",
            ImageUrl = "/Animals/Sam.JPG",
            CategorySlug = "animals-and-people",
            Width = 1512,
            Height = 2016,
            Year = 2023,
            Price = 1600,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Snail Kite",
            Slug = "snail-kite",
            Description = "A snail kite bird",
            ImageUrl = "/Animals/Snail_Kite_.jpg",
            CategorySlug = "animals-and-people",
            Width = 4800,
            Height = 3600,
            Year = 2023,
            Price = 1800,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Snowy Owl",
            Slug = "snowy-owl",
            Description = "A snowy owl",
            ImageUrl = "/Animals/SnowyOwl.jpg",
            CategorySlug = "animals-and-people",
            Width = 2700,
            Height = 2160,
            Year = 2023,
            Price = 1700,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Spiny Lobster",
            Slug = "spiny-lobster",
            Description = "A spiny lobster",
            ImageUrl = "/Animals/Spiny_Lobster.jpg",
            CategorySlug = "animals-and-people",
            Width = 4800,
            Height = 3600,
            Year = 2023,
            Price = 1500,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Tigers",
            Slug = "tigers",
            Description = "Majestic tigers",
            ImageUrl = "/Animals/Tigers.jpg",
            CategorySlug = "animals-and-people",
            Width = 2000,
            Height = 1600,
            Year = 2023,
            Price = 2000,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "To the Light",
            Slug = "to-the-light",
            Description = "Moving to the light",
            ImageUrl = "/Animals/To_the_Light.jpg",
            CategorySlug = "animals-and-people",
            Width = 3024,
            Height = 4032,
            Year = 2023,
            Price = 1900,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "What's for Dinner",
            Slug = "whats-for-dinner",
            Description = "What's for dinner",
            ImageUrl = "/Animals/Whats_for_dinner.jpg",
            CategorySlug = "animals-and-people",
            Width = 480,
            Height = 640,
            Year = 2023,
            Price = 1300,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Wild White Majesty",
            Slug = "wild-white-majesty",
            Description = "Wild white majesty",
            ImageUrl = "/Animals/Wild_White_Majesty.jpg",
            CategorySlug = "animals-and-people",
            Width = 2969,
            Height = 2227,
            Year = 2023,
            Price = 1800,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "ZsaZsa",
            Slug = "zsazsa",
            Description = "Portrait of ZsaZsa",
            ImageUrl = "/Animals/ZsaZsa.jpeg",
            CategorySlug = "animals-and-people",
            Width = 1512,
            Height = 2016,
            Year = 2023,
            Price = 1600,
            IsAvailable = true
        }
    };

    public static readonly IEnumerable<PaintingSeed> LandscapesAndCityscapes = new[]
    {
        new PaintingSeed
        {
            Title = "Aspens",
            Slug = "aspens",
            Description = "Beautiful aspen trees in autumn",
            ImageUrl = "/Landscapes-and-Cityscapes/Aspens.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 1182,
            Height = 788,
            Year = 2023,
            Price = 1200,
            IsAvailable = true,
            IsNew = true
        },
        new PaintingSeed
        {
            Title = "Bahia Honda",
            Slug = "bahia-honda",
            Description = "The scenic Bahia Honda bridge",
            ImageUrl = "/Landscapes-and-Cityscapes/Bahia_Honda_.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 4000,
            Height = 3000,
            Year = 2023,
            Price = 1500,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "BakedGoods",
            Slug = "bakedgoods",
            Description = "A charming bakery scene",
            ImageUrl = "/Landscapes-and-Cityscapes/BakedGoods.JPG",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 3000,
            Height = 4000,
            Year = 2023,
            Price = 1100,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Banyans",
            Slug = "banyans",
            Description = "Majestic banyan trees",
            ImageUrl = "/Landscapes-and-Cityscapes/Banyans.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 4000,
            Height = 3000,
            Year = 2023,
            Price = 1400,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Brady & Duke",
            Slug = "brady-duke",
            Description = "Friends Brady and Duke together",
            ImageUrl = "/Landscapes-and-Cityscapes/Brady_&_Duke.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 4000,
            Height = 3000,
            Year = 2023,
            Price = 1300,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Bridge world",
            Slug = "bridge-world",
            Description = "A world connected by bridges",
            ImageUrl = "/Landscapes-and-Cityscapes/Bridge_world.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 4000,
            Height = 3000,
            Year = 2023,
            Price = 1600,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Castelvetore",
            Slug = "castelvetore",
            Description = "The historic town of Castelvetore",
            ImageUrl = "/Landscapes-and-Cityscapes/Castelvetore_.JPG",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 4000,
            Height = 3000,
            Year = 2023,
            Price = 1700,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Cobblestone Way",
            Slug = "cobblestone-way",
            Description = "A picturesque cobblestone street",
            ImageUrl = "/Landscapes-and-Cityscapes/Cobblestone_Way.JPG",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 3000,
            Height = 4000,
            Year = 2023,
            Price = 1200,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Forest Mystery",
            Slug = "forest-mystery",
            Description = "Mysterious depths of the forest",
            ImageUrl = "/Landscapes-and-Cityscapes/Forest_Mystery.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 4800,
            Height = 6000,
            Year = 2023,
            Price = 1900,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Hydrangea Time",
            Slug = "hydrangea-time",
            Description = "Beautiful hydrangeas in bloom",
            ImageUrl = "/Landscapes-and-Cityscapes/Hydrangea_Time.jpeg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 4000,
            Height = 3000,
            Year = 2023,
            Price = 1100,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Italian Church",
            Slug = "italian-church",
            Description = "A charming Italian church",
            ImageUrl = "/Landscapes-and-Cityscapes/Italian_Church.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 1469,
            Height = 1959,
            Year = 2023,
            Price = 1500,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Mine (1)",
            Slug = "mine-1",
            Description = "An old mine site",
            ImageUrl = "/Landscapes-and-Cityscapes/Mine_(1).jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 3000,
            Height = 4000,
            Year = 2023,
            Price = 1400,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "New Inhabitants",
            Slug = "new-inhabitants",
            Description = "New inhabitants of an old place",
            ImageUrl = "/Landscapes-and-Cityscapes/New_Inhabitants.jpeg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 3000,
            Height = 4000,
            Year = 2023,
            Price = 1300,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Rainbow River1",
            Slug = "rainbow-river1",
            Description = "The colorful Rainbow River",
            ImageUrl = "/Landscapes-and-Cityscapes/Rainbow_River1.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 4000,
            Height = 3000,
            Year = 2023,
            Price = 1600,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Sedona",
            Slug = "sedona",
            Description = "The red rocks of Sedona",
            ImageUrl = "/Landscapes-and-Cityscapes/Sedona.JPG",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 3000,
            Height = 4000,
            Year = 2023,
            Price = 1800,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "The Path",
            Slug = "the-path",
            Description = "A winding path through nature",
            ImageUrl = "/Landscapes-and-Cityscapes/The_Path.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 3000,
            Height = 4000,
            Year = 2023,
            Price = 1500,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "The Pond",
            Slug = "the-pond",
            Description = "A serene pond scene",
            ImageUrl = "/Landscapes-and-Cityscapes/The_Pond.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 3000,
            Height = 4000,
            Year = 2023,
            Price = 1400,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Window Jigsaw",
            Slug = "window-jigsaw",
            Description = "A jigsaw puzzle view from the window",
            ImageUrl = "/Landscapes-and-Cityscapes/Window_Jigsaw.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 3000,
            Height = 4000,
            Year = 2023,
            Price = 1200,
            IsAvailable = true
        }
    };

    public static readonly IEnumerable<PaintingSeed> Flowers = new[]
    {
        new PaintingSeed
        {
            Title = "Bird of Paradise",
            Slug = "bird-of-paradise",
            Description = "Stunning bird of paradise flowers",
            ImageUrl = "/Flowers/Bird_of_Paradise.jpg",
            CategorySlug = "flowers",
            Width = 20000,
            Height = 15000,
            Year = 2023,
            Price = 1800,
            IsAvailable = true,
            IsNew = true
        },
        new PaintingSeed
        {
            Title = "Bumblebee",
            Slug = "bumblebee",
            Description = "A busy bumblebee",
            ImageUrl = "/Flowers/Bumblebee.jpg",
            CategorySlug = "flowers",
            Width = 2016,
            Height = 1512,
            Year = 2023,
            Price = 1500,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Coneflowers",
            Slug = "coneflowers",
            Description = "Beautiful coneflowers",
            ImageUrl = "/Flowers/Coneflowers.jpg",
            CategorySlug = "flowers",
            Width = 1200,
            Height = 900,
            Year = 2023,
            Price = 1400,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Daffodils",
            Slug = "daffodils",
            Description = "Spring daffodils",
            ImageUrl = "/Flowers/Daffodils.jpg",
            CategorySlug = "flowers",
            Width = 3024,
            Height = 4032,
            Year = 2023,
            Price = 1600,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Flowers for Dee",
            Slug = "flowers-for-dee",
            Description = "Special flowers for Dee",
            ImageUrl = "/Flowers/Flowers_for_Dee.jpg",
            CategorySlug = "flowers",
            Width = 2016,
            Height = 1512,
            Year = 2023,
            Price = 1700,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Honey Bee",
            Slug = "honey-bee",
            Description = "A honey bee at work",
            ImageUrl = "/Flowers/Honey_bee.jpeg",
            CategorySlug = "flowers",
            Width = 4032,
            Height = 3024,
            Year = 2023,
            Price = 1500,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Milkweed Hummingbird",
            Slug = "milkweed-hummingbird",
            Description = "Hummingbird at milkweed",
            ImageUrl = "/Flowers/MilkweedHummingbird.jpg",
            CategorySlug = "flowers",
            Width = 2016,
            Height = 1512,
            Year = 2023,
            Price = 1800,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Monarch Beginnings",
            Slug = "monarch-beginnings",
            Description = "Monarch butterfly beginnings",
            ImageUrl = "/Flowers/Monarch_Beginnings.JPG",
            CategorySlug = "flowers",
            Width = 1512,
            Height = 2016,
            Year = 2023,
            Price = 1600,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Royal Poinciana",
            Slug = "royal-poinciana",
            Description = "Royal poinciana flowers",
            ImageUrl = "/Flowers/Royal_Poinciana.jpg",
            CategorySlug = "flowers",
            Width = 2016,
            Height = 1512,
            Year = 2023,
            Price = 1700,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Squash Blossoms",
            Slug = "squash-blossoms",
            Description = "Yellow squash blossoms",
            ImageUrl = "/Flowers/Squash_blossoms1.JPG",
            CategorySlug = "flowers",
            Width = 3024,
            Height = 4032,
            Year = 2023,
            Price = 1500,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "The Bee",
            Slug = "the-bee",
            Description = "A bee in bloom",
            ImageUrl = "/Flowers/The_Bee.jpg",
            CategorySlug = "flowers",
            Width = 2016,
            Height = 1512,
            Year = 2023,
            Price = 1400,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Water Lilies",
            Slug = "water-lilies",
            Description = "Peaceful water lilies on a pond",
            ImageUrl = "/Flowers/Water_lilies.jpg",
            CategorySlug = "flowers",
            Width = 3600,
            Height = 7200,
            Year = 2023,
            Price = 2000,
            IsAvailable = true
        }
    };

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
} */
namespace ServerApp.Infrastructure.SeedData;

public static class AnimalsSeedData
{
    public static readonly IEnumerable<PaintingSeed> Animals = new[]
    {
        new PaintingSeed
        {
            Title = "Abby's Horse",
            Slug = "abbys-horse",
            Description = "Oil on canvas. Gifted to my niece who I captured jumping with her horse at an equestrian event.",
            ImageUrl = "/images/high-res/Abbys_Horse.jpg",
            ThumbnailUrl = "/images/thumbnail/Abbys_Horse_.jpg",
            CategorySlug = "animals",
            Width = 12,
            Height = 16,
            IsAvailable = false
        },
        new PaintingSeed
        {
            Title = "Water Angel",
            Slug = "water-angel",
            Description = "Framed oil on canvas. Angelfish are loved by aquarium enthusiasts but live in South America (Angel fish and purple fan fish depicted).",
            ImageUrl = "/images/high-res/Water_Angel.jpg",
            ThumbnailUrl = "/images/thumbnail/Water_Angel_.jpg",
            CategorySlug = "animals",
            Width = 26,
            Height = 22,
            Price = 850,
            IsAvailable = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Cruising",
            Slug = "cruising",
            Description = "Framed oil on canvas. The Green Sea Turtle is found throughout the tropical and subtropical seas. It is named by the green fat found beneath its carapace due to its diet of seagrass.",
            ImageUrl = "/images/high-res/Cruising.jpg",
            ThumbnailUrl = "/images/thumbnail/Cruising_.jpg",
            CategorySlug = "animals",
            Width = 18,
            Height = 14,
            Price = 680,
            IsAvailable = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Big Yellow",
            Slug = "big-yellow",
            Description = "Framed oil on canvas. Big Yellow is part of a diptych of the same size called Ocean Butterflies (Together $2000). Can be purchased separately.",
            ImageUrl = "/images/high-res/Big_Yellow.jpg",
            ThumbnailUrl = "/images/thumbnail/Big_Yellow_.jpg",
            CategorySlug = "animals",
            Width = 25,
            Height = 25,
            Price = 1200,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Ocean Butterflies",
            Slug = "ocean-butterflies",
            Description = "Framed oil on canvas. Ocean Butterflies is part of a diptych of the same size named Big Yellow (Together $2000). Can be purchased separately.",
            ImageUrl = "/images/high-res/Ocean_butterflies.jpg",
            CategorySlug = "animals",
            Width = 25,
            Height = 25,
            Price = 1200,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Ever Vigilant",
            Slug = "ever-vigilant",
            Description = "Framed oil on canvas. The \"hoot owl\" is a Barred Owl who inhabits the northern woodlands.",
            ImageUrl = "/images/high-res/Ever_Vigilant.jpg",
            ThumbnailUrl = "/images/thumbnail/Ever_Vigilant_.jpg",
            CategorySlug = "animals",
            Width = 18,
            Height = 22,
            IsAvailable = false
        },
        new PaintingSeed
        {
            Title = "Fairy-Wrens",
            Slug = "fairy-wrens",
            Description = "Framed oil on canvas. They are white-winged fairy-wrens from inland Australia. The blue bird is the dominant male. The three birds on the left are juveniles. The bird immediately to right of the blue bird is an uncolored male and the other two are females. This description is from an Australian ornithologist who photographed them at night in the shrub.",
            ImageUrl = "/images/high-res/Fairy_Wrens.jpg",
            ThumbnailUrl = "/images/thumbnail/Fairy_Wrens_.jpg",
            CategorySlug = "animals",
            Width = 30,
            Height = 18,
            IsAvailable = false,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "The Glider",
            Slug = "the-glider",
            Description = "Framed oil on canvas. The Green Sea Turtle is found throughout the tropical and subtropical seas. It is named by the green fat found beneath its carapace due to its diet of seagrass.",
            ImageUrl = "/images/high-res/Green_Turtle.jpg",
            CategorySlug = "animals",
            Width = 20,
            Height = 28,
            IsAvailable = false
        },
        new PaintingSeed
        {
            Title = "Leatherback",
            Slug = "leatherback",
            Description = "Framed on canvas. The leatherback is the largest of all living turtles and the heaviest. It has existed since the first turtles on earth, and its carapace is covered by oily flesh and flexible, leather-like skin, for which it is named. The leatherback is an endangered species.",
            ImageUrl = "/images/high-res/Leatherback.jpg",
            ThumbnailUrl = "/images/thumbnail/Leatherback_.jpg",
            CategorySlug = "animals",
            Width = 30,
            Height = 25,
            Price = 1600,
            IsAvailable = true,
            IsNew = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Buddies",
            Slug = "buddies",
            Description = "Framed oil on canvas. Manatees live in marshy coastal areas and rivers, where they are a threatened species mainly due to human activities. Swimming in a river with them is an amazing experience because they are so gentle and curious herbivores.",
            ImageUrl = "/images/high-res/Buddies.jpg",
            ThumbnailUrl = "/images/thumbnail/Buddies_.jpg",
            CategorySlug = "animals",
            Width = 30,
            Height = 25,
            Price = 1600,
            IsAvailable = true,
            IsNew = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Meadow",
            Slug = "meadow",
            Description = "Oil on canvas. Meadow is my son and daughter-in-law's dog and now lives in Tennessee. She chose them when they saw her in a shelter. What a fantastic grand-dog!",
            ImageUrl = "/images/high-res/Meadow.jpg",
            CategorySlug = "animals",
            Width = 16,
            Height = 20,
            IsAvailable = false
        },
        new PaintingSeed
        {
            Title = "My Green Visitor",
            Slug = "my-green-visitor",
            Description = "Framed oil on canvas. The frog was so well camouflaged in the greenery by my door.",
            ImageUrl = "/images/high-res/My_Green_Visitor.jpg",
            CategorySlug = "animals",
            Width = 15,
            Height = 12,
            Price = 350,
            IsAvailable = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Octopus Purple",
            Slug = "octopus-purple",
            Description = "Oil on canvas. Octopuses are highly intelligent and only live up to 4 years. They can change the color and texture of their skin.",
            ImageUrl = "/images/high-res/Octopus_Purple.jpg",
            CategorySlug = "animals",
            Width = 24,
            Height = 36,
            IsAvailable = false
        },
        new PaintingSeed
        {
            Title = "Painted Bunting",
            Slug = "painted-bunting",
            Description = "Framed oil on canvas. The painted bunting is considered one of the most beautiful birds that live in North America, and will catch your eye wherever you are.",
            ImageUrl = "/images/high-res/Painted_Bunting.jpg",
            ThumbnailUrl = "/images/thumbnail/Painted_Bunting_.jpg",
            CategorySlug = "animals",
            Width = 10,
            Height = 12,
            IsAvailable = false
        },
        new PaintingSeed
        {
            Title = "Peekaboo",
            Slug = "peekaboo",
            Description = "Oil on gallery wrapped canvas. The burrowing owl is a threatened species in Florida.",
            ImageUrl = "/images/high-res/Peekaboo.jpg",
            ThumbnailUrl = "/images/thumbnail/Peekaboo_.jpg",
            CategorySlug = "animals",
            Width = 20,
            Height = 15,
            Price = 700,
            IsAvailable = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Pink Floy",
            Slug = "pink-floy",
            Description = "Oil on gallery wrapped canvas. The roseate spoonbill was almost driven to extinction for its beautiful pink plumage.",
            ImageUrl = "/images/high-res/Pink_Floy.jpg",
            ThumbnailUrl = "/images/thumbnail/Pink_Floy_.jpg",
            CategorySlug = "animals",
            Width = 30,
            Height = 24,
            IsAvailable = false,
            IsNew = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Sam",
            Slug = "sam",
            Description = "Oil on canvas. This Jack Russell named Sam was loved by a good friend who found him in the basement abandoned by people who had moved out of the house. She adopted him and after many years with her he became blind but kept his amazing little personality.",
            ImageUrl = "/images/high-res/Sam.jpg",
            ThumbnailUrl = "/images/thumbnail/Sam_.jpg",
            CategorySlug = "animals",
            Width = 12,
            Height = 16,
            IsAvailable = false
        },
        new PaintingSeed
        {
            Title = "Snail Kite",
            Slug = "snail-kite",
            Description = "Framed oil on art panel. The snail kite is a locally endangered species in the Florida Everglades with a population of less than 400 breeding pairs. It is amazing to see them drop their food, apple snails, onto a hard surface from a high vantage point to crack open the shells.",
            ImageUrl = "/images/high-res/Snail_Kite.jpg",
            ThumbnailUrl = "/images/thumbnail/Snail_Kite_.jpg",
            CategorySlug = "animals",
            Width = 24,
            Height = 20,
            Price = 550,
            IsAvailable = true,
            IsNew = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Snowy Owl",
            Slug = "snowy-owl",
            Description = "Framed oil on canvas. The snowy owl is a very large bird of the Arctic regions, hence the mainly white color of both males and females. In this painting taken from a photo she has numerous black spots, which suggests she is female. Snowy owls differ from other owls in that they are nomadic and often hunt during the day.",
            ImageUrl = "/images/high-res/Snowy_Owl.jpg",
            ThumbnailUrl = "/images/thumbnail/Snowy_Owl_.jpg",
            CategorySlug = "animals",
            Width = 26,
            Height = 22,
            Price = 750,
            IsAvailable = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Spiny Lobster",
            Slug = "spiny-lobster",
            Description = "Framed oil on canvas. Spiny lobsters love warm water such as the Caribbean and can detect the Earth's magnetic field. They are also able to screech by rubbing their antennae against their exoskeleton to scare away predators.",
            ImageUrl = "/images/high-res/Spiny_Lobster.jpg",
            ThumbnailUrl = "/images/thumbnail/Spiny_Lobster_.jpg",
            CategorySlug = "animals",
            Width = 14,
            Height = 10,
            IsAvailable = false,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Family Gathering",
            Slug = "family-gathering",
            Description = "Framed oil on canvas. Tigers are endangered mostly because of habitat destruction and poaching. They live mainly in Asia. How could I not want to paint such majestic creatures?",
            ImageUrl = "/images/high-res/Family_Gathering.jpg",
            ThumbnailUrl = "/images/thumbnail/Family_Gathering_.jpg",
            CategorySlug = "animals",
            Width = 25,
            Height = 21,
            Price = 950,
            IsAvailable = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "What's for Dinner?",
            Slug = "whats-for-dinner",
            Description = "Framed oil on canvas. Catbirds often line their nests with soft materials. In my backyard, a catbird kept on trying to fly and grab my mother's wispy gray hair on her head. When I saw this photo I had to paint it in memory of my mother.",
            ImageUrl = "/images/high-res/Whats_for_dinner.jpg",
            ThumbnailUrl = "/images/thumbnail/Whats_for_dinner_.jpg",
            CategorySlug = "animals",
            Width = 14,
            Height = 18,
            Price = 730,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "ZsaZsa",
            Slug = "zsazsa",
            Description = "Framed oil on canvas. My neighbor's tabby adopted us when his family moved away. They built a new house with a fenced backyard and took him back with them. He was once an indoor cat but after tasting the outdoors, would not stay indoors even after numerous run-ins with bobcats which eventually took his life at a very old age.",
            ImageUrl = "/images/high-res/ZsaZsa.jpg",
            ThumbnailUrl = "/images/thumbnail/ZsaZsa.jpg",
            CategorySlug = "animals",
            Width = 12,
            Height = 16,
            IsAvailable = false
        },
        new PaintingSeed
        {
            Title = "Hawksbill Turtle",
            Slug = "hawksbill-turtle",
            Description = "Framed oil on canvas. The hawksbill turtle is critically endangered. I had the exceptional experience to swim with one while I was with my family in the Caribbean Sea. Hawksbill turtles became endangered because their shells were used for decorative purposes (tortoise shell combs, etc.).",
            ImageUrl = "/images/high-res/Hawksbill_Turtle.jpg",
            ThumbnailUrl = "/images/thumbnail/Hawksbill_Turtle_.jpg",
            CategorySlug = "animals",
            Width = 33,
            Height = 25,
            Price = 2500,
            IsAvailable = true,
            IsNew = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Coral Reef Color",
            Slug = "coral-reef-color",
            Description = "Framed oil on canvas. I photographed these fish in an aquarium in the Florida Keys.",
            ImageUrl = "/images/high-res/Coral_Reef_Color.jpg",
            ThumbnailUrl = "/images/thumbnail/Coral_Reef_Color_.jpg",
            CategorySlug = "animals",
            Width = 30,
            Height = 30,
            Price = 2000,
            IsAvailable = true,
            IsNew = true
        }
    };
}
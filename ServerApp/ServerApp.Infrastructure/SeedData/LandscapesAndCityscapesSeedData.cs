namespace ServerApp.Infrastructure.SeedData;

public static class LandscapesAndCityscapesSeedData
{
    public static readonly IEnumerable<PaintingSeed> LandscapesAndCityscapes = new[]
    {
        new PaintingSeed
        {
            Title = "Aspens",
            Slug = "aspens",
            Description = "Oil on canvas. Aspens trees form large colonies and connect to each other through their underground roots.",
            ImageUrl = "/images/high-res/Aspens.jpg",
            ThumbnailUrl = "/images/thumbnail/Aspens_.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 26,
            Height = 20,
            IsAvailable = false,
            IsNew = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Bahia Honda",
            Slug = "bahia-honda",
            Description = "Oil on canvas panel. This scene was painted in Bahia Honda state park in the Florida Keys.",
            ImageUrl = "/images/high-res/Bahia_Honda.jpg",
            ThumbnailUrl = "/images/thumbnail/Bahia_Honda_.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 40,
            Height = 30,
            IsAvailable = false,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Baked Goods",
            Slug = "baked-goods",
            Description = "Unframed oil on canvas. The aroma coming from the open door of the bakery was delicious.",
            ImageUrl = "/images/high-res/BakedGoods.jpg",
            ThumbnailUrl = "/images/thumbnail/BakedGoods_.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 16,
            Height = 20,
            Price = 700,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Banyons",
            Slug = "banyons",
            Description = "Framed oil on canvas. Banyon trees are large tropical fig trees native to India where it holds religious significance. Their large aerial roots grow from branches to form new trunks. They are also found in South Florida.",
            ImageUrl = "/images/high-res/Banyans.jpg",
            ThumbnailUrl = "/images/thumbnail/Banyans_.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 20,
            Height = 16,
            Price = 800,
            IsAvailable = true,
            IsNew = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Tropical Boardwalk",
            Slug = "tropical-boardwalk",
            Description = "Framed oil on canvas. Glistening with rain from the tropical climate, the walk is magical.",
            ImageUrl = "/images/high-res/Tropical_Boardwalk.jpg",
            ThumbnailUrl = "/images/thumbnail/Tropical_Boardwalk_.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 18,
            Height = 14,
            Price = 630,
            IsAvailable = true,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Castelvetore",
            Slug = "castelvetore",
            Description = "Oil on gallery wrapped canvas. A friend's family is from Castelvetore in Italy and he wanted 3 paintings (triptych) for his dining room. Italian Church and Cobblestone Way belong together in the triptych.",
            ImageUrl = "/images/high-res/Castelvetore.jpg",
            ThumbnailUrl = "/images/thumbnail/Castelvetore_.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 30,
            Height = 24,
            IsAvailable = false,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Cobblestone Way",
            Slug = "cobblestone-way",
            Description = "Oil on gallery wrapped canvas. A friend's family is from Castelvetore in Italy and he wanted 3 paintings (triptych) for his dining room. This cobblestone street runs through the town. Italian Church and Castelvetore belong together in the triptych.",
            ImageUrl = "/images/high-res/Cobblestone_Way.jpg",
            ThumbnailUrl = "/images/thumbnail/Cobblestone_Way_.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 20,
            Height = 24,
            IsAvailable = false
        },
        new PaintingSeed
        {
            Title = "Italian Church",
            Slug = "italian-church",
            Description = "Oil on gallery wrapped canvas. A friend's family is from Castelvetore in Italy and he wanted 3 paintings (triptych) for his dining room. Italian Church and Castelvetore belong together in the triptych. The church is celebrating Madonna delle Grazie.",
            ImageUrl = "/images/high-res/Italian_Church.jpg",
            ThumbnailUrl = "/images/thumbnail/Italian_Church_.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 20,
            Height = 24,
            IsAvailable = false
        },
        new PaintingSeed
        {
            Title = "Spanish Moss Forest",
            Slug = "spanish-moss-forest",
            Description = "Framed oil on canvas. Spanish moss is a flowering plant in the pineapple family. It is an epiphyte which gets its nutrients from the air but is attached to trees for support.",
            ImageUrl = "/images/high-res/Spanish_Moss_Forest.jpg",
            ThumbnailUrl = "/images/thumbnail/Spanish_Moss_Forest_.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 19,
            Height = 23,
            Price = 750,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Hydrangea Time",
            Slug = "hydrangea-time",
            Description = "Oil on canvas. Hydrangeas are magnificent, blooming in the summer at my friend's home.",
            ImageUrl = "/images/high-res/Hydrangea_Time.jpg",
            ThumbnailUrl = "/images/thumbnail/Hydrangea_Time_.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 18,
            Height = 14,
            IsAvailable = false,
            IsLandscape = true
        },
        new PaintingSeed
        {
            Title = "Mine",
            Slug = "mine",
            Description = "Oil on canvas. Ice cream tastes so good, it can be difficult to share.",
            ImageUrl = "/images/high-res/Mine.jpg",
            ThumbnailUrl = "/images/thumbnail/Mine_.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 16,
            Height = 20,
            Price = 400,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "New Inhabitants",
            Slug = "new-inhabitants",
            Description = "Oil on canvas panel. Darien, Georgia had this old, abandoned building that was once a coastal warehouse. It is now restored and has become a brew pub.",
            ImageUrl = "/images/high-res/New_Inhabitants.jpg",
            ThumbnailUrl = "/images/thumbnail/New_Inhabitants_.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 16,
            Height = 20,
            Price = 700,
            IsAvailable = true,
            IsNew = true
        },
        new PaintingSeed
        {
            Title = "The Path",
            Slug = "the-path",
            Description = "Framed oil on canvas. This is a beautiful path in Violet Curry park in Lutz Florida where my family used to walk with my granddog.",
            ImageUrl = "/images/high-res/The_Path.jpg",
            ThumbnailUrl = "/images/thumbnail/The_Path.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 28,
            Height = 40,
            Price = 1800,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Pond Color",
            Slug = "pond-color",
            Description = "Oil on canvas panel. The calm life of a pond supports so many colorful plants for plein air painting.",
            ImageUrl = "/images/high-res/Pond_Color.jpg",
            ThumbnailUrl = "/images/thumbnail/Pond_Color_.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 12,
            Height = 16,
            Price = 250,
            IsAvailable = true
        }
    };
}
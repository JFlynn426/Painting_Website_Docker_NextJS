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
            ImageUrl = "/Landscapes-Full/Aspens.jpg",
            ThumbnailUrl = "/Landscapes-Thumbnail/Aspens_.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 26,
            Height = 20,
            IsAvailable = false
        },
        new PaintingSeed
        {
            Title = "Bahia Honda",
            Slug = "bahia-honda",
            Description = "Oil on canvas panel. This scene was painted in Bahia Honda state park in the Florida Keys.",
            ImageUrl = "/Landscapes-Full/Bahia_Honda.jpg",
            ThumbnailUrl = "/Landscapes-Thumbnail/Bahia_Honda_.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 40,
            Height = 30,
            IsAvailable = false
        },
        new PaintingSeed
        {
            Title = "Baked Goods",
            Slug = "baked-goods",
            Description = "Unframed oil on canvas. The aroma coming from the open door of the bakery was delicious.",
            ImageUrl = "/Landscapes-Full/BakedGoods.jpg",
            ThumbnailUrl = "/Landscapes-Thumbnail/BakedGoods_.jpg",
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
            ImageUrl = "/Landscapes-Full/Banyans.jpg",
            ThumbnailUrl = "/Landscapes-Thumbnail/Banyans_.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 20,
            Height = 16,
            Price = 800,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Tropical Boardwalk",
            Slug = "tropical-boardwalk",
            Description = "Framed oil on canvas. Glistening with rain from the tropical climate, the walk is magical.",
            ImageUrl = "/Landscapes-Full/Tropical_Boardwalk.jpg",
            ThumbnailUrl = "/Landscapes-Thumbnail/Tropical_Boardwalk_.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 18,
            Height = 14,
            Price = 630,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "Castelvetore",
            Slug = "castelvetore",
            Description = "Oil on gallery wrapped canvas. A friend's family is from Castelvetore in Italy and he wanted 3 paintings (triptych) for his dining room. Italian Church and Cobblestone Way belong together in the triptych.",
            ImageUrl = "/Landscapes-Full/Castelvetore.jpg",
            ThumbnailUrl = "/Landscapes-Thumbnail/Castelvetore_.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 30,
            Height = 24,
            IsAvailable = false
        },
        new PaintingSeed
        {
            Title = "Cobblestone Way",
            Slug = "cobblestone-way",
            Description = "Oil on gallery wrapped canvas. A friend's family is from Castelvetore in Italy and he wanted 3 paintings (triptych) for his dining room. This cobblestone street runs through the town. Italian Church and Castelvetore belong together in the triptych.",
            ImageUrl = "/Landscapes-Full/Cobblestone_Way.jpg",
            ThumbnailUrl = "/Landscapes-Thumbnail/Cobblestone_Way_.jpg",
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
            ImageUrl = "/Landscapes-Full/Italian_Church.jpg",
            ThumbnailUrl = "/Landscapes-Thumbnail/Italian_Church_.jpg",
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
            ImageUrl = "/Landscapes-Full/Spanish_Moss_Forest.jpg",
            ThumbnailUrl = "/Landscapes-Thumbnail/Spanish_Moss_Forest_.jpg",
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
            ImageUrl = "/Landscapes-Full/Hydrangea_Time.jpg",
            ThumbnailUrl = "/Landscapes-Thumbnail/Hydrangea_Time_.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 18,
            Height = 14,
            IsAvailable = false
        },
        new PaintingSeed
        {
            Title = "Mine",
            Slug = "mine",
            Description = "Oil on canvas. Ice cream tastes so good, it can be difficult to share.",
            ImageUrl = "/Landscapes-Full/Mine.jpg",
            ThumbnailUrl = "/Landscapes-Thumbnail/Mine_.jpg",
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
            ImageUrl = "/Landscapes-Full/New_Inhabitants.jpg",
            ThumbnailUrl = "/Landscapes-Thumbnail/New_Inhabitants_.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 16,
            Height = 20,
            Price = 700,
            IsAvailable = true
        },
        new PaintingSeed
        {
            Title = "The Path",
            Slug = "the-path",
            Description = "Framed oil on canvas. This is a beautiful path in Violet Curry park in Lutz Florida where my family used to walk with my granddog.",
            ImageUrl = "/Landscapes-Full/The_Path.jpg",
            ThumbnailUrl = "/Landscapes-Thumbnail/The_Path.jpg",
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
            ImageUrl = "/Landscapes-Full/Pond_Color.jpg",
            ThumbnailUrl = "/Landscapes-Thumbnail/Pond_Color_.jpg",
            CategorySlug = "landscapes-and-cityscapes",
            Width = 12,
            Height = 16,
            Price = 250,
            IsAvailable = true
        }
    };
}
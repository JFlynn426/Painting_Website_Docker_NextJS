namespace ServerApp.Infrastructure.SeedData;

/// <summary>
/// Seed data for the PageContents table.
/// 
/// Table Schema:
/// - Id (uniqueidentifier, PK, ValueGeneratedOnAdd)
/// - Address (nvarchar(200), Required, Unique Index)
/// - Title (nvarchar(200), Required)
/// - Content (nvarchar(max), Required)
/// 
/// This data matches the client-side models in clientapp/src/app/models/pageContent.ts
/// to ensure consistency between frontend and backend data.
/// </summary>
public static class PageContentsSeedData
{
    public static readonly List<PageContentSeed> PageContents = new()
    {
        new PageContentSeed
        {
            Address = "home",
            Title = null,
            Content = @"From the sun-drenched sidewalks of New York City to the luminous waters of South Florida, my world-wide travels for work and pleasure led me to the canvas. As a child of immigrants, I haunted grand art museums and Central Park, and carried a gift for color and light long before science claimed me. For decades, I channeled my visual insight into the world of science, becoming an Emeritus Professor at the University of Connecticut and illustrating a book on personalized medicine with my own drawings. Following the passion I carried my whole life, I returned to the canvas with a lifetime of seeing, studying, and loving the natural world around me.

My oil paintings are acts of devotion — to nature, to memory, and to the fleeting moments that define life on our planet. Rooted in a deep commitment to environmental conservation and an abiding love for the remarkable ecosystems of South Florida, my work seeks to tell a story. Each painting visualizes the light and color of what came before and the quiet anticipation of what comes next, inviting the viewer to step inside a living, breathing world. My goal is to show that art and conservation become one and the same.",
            PhotoUrls = new[]
            {
                "/Seascapes-Thumbnail/Wind_and_Water_.jpg",
                "/Animals-Thumbnail/Buddies_.jpg",
                "/Seascapes-Thumbnail/Solitude.jpg",
                "/Landscapes-Thumbnail/Aspens_.jpg",
                "/Animals-Thumbnail/Leatherback_.jpg",
                "/Flowers-Thumbnail/Bird_of_Paradise_.jpg"
            }
        },
        new PageContentSeed
        {
            Address = "about",
            Title = "About Gloria Gronowicz",
            Content = @"As my early life was filled with city streets and art museums in which I played, observed and admired, I demonstrated a strong aptitude for drawing and painting. I received annual recognition for my artwork and earned the only Art Award upon graduating high school. Growing up as the child of immigrants in New York City, my father encouraged me to pursue a career that would provide more financial stability instead of the competitive world of art in a big city. I went on to earn a Ph.D. in biology from Columbia University and built a career as a scientist, educator, and ultimately an Emeritus Professor at the University of Connecticut.

Visual analysis played a central role in my scientific work. I produced my own images for scientific publications, and authored a book on personalized medicine, for which I created nearly all the illustrations. I published many scientific, peer-reviewed articles on my research. Following my retirement from science, I have been able to fully dedicate myself to my lifelong passion for art that tells a story of life on our planet.

My paintings are inspired by a deep love of nature and a strong commitment to environmental conservation. I am actively involved in conservation efforts in South Florida. I consider it a privilege to live in such a beautiful and unique natural setting. These values strongly influence my artistic subject matter. My oil paintings emphasize color and light to capture the beauty of the natural world. Most of my paintings tell a story of the events leading to that captured moment, and what will happen next.",
            PhotoUrls = new[] { "/Other/AboutPagePhoto.JPG" }
        },
        new PageContentSeed
        {
            Address = "galleries",
            Title = "Galleries",
            Content = @"[align:center]

<strong>Emerging Artist at Stuart Artfest 2024</strong>

<strong>Lighthouse Art Center</strong>
Annual Art Show
373 Tequesta Drive
Tequesta, FL 3349

<strong>Palm City Art Association</strong>
Annual Art Show at Cumming Library
2551 SW Matheson Avenue
Palm City FL. 34990

<strong>Cleveland Clinic Health and Wellness Center</strong>
3066 SW Martin Downs Blvd.
Palm City, FL 34990

<strong>Martin Arts</strong>
Annual Art Show
80 SE Ocean Blvd
Stuart, FL 34994

<strong>Hammock Creek Golf Club</strong>
2400 SW Golden Bear Way
Palm City, FL 34990"
        },
        new PageContentSeed
        {
            Address = "contact",
            Title = "Contact",
            Content = @"[align:center]
To inquire about purchasing a painting or ordering prints, please contact Gloria Gronowicz.

Email inquiries are preferred for detailed questions about specific artworks.

<strong>Email:</strong> gloriagronowicz@gmail.com

<strong>Phone:</strong> (860) 670-0799

I look forward to hearing from you and discussing how my art can bring beauty to your home or office."
        }
    };
}

/// <summary>
/// Represents seed data for page content.
/// Matches the PageContentDto structure.
/// </summary>
public class PageContentSeed
{
    public string Address { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string Content { get; set; } = string.Empty;
    public string[]? PhotoUrls { get; set; }
}
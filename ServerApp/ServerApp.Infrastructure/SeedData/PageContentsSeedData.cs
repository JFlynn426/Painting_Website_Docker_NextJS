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
            Content = @"<div style='text-align: left; max-width: 800px; margin: 0 auto;'>
            <p style='margin-bottom: 16px;'>From the sun-drenched sidewalks of New York City to the luminous waters of South Florida, my world-wide travels for work and pleasure led me to the canvas. As a child of immigrants, I haunted grand art museums and Central Park, and carried a gift for color and light long before science claimed me. For decades, I channeled my visual insight into the world of science, becoming an Emeritus Professor at the University of Connecticut and illustrating a book on personalized medicine with my own drawings. Following the passion I carried my whole life, I returned to the canvas with a lifetime of seeing, studying, and loving the natural world around me.</p>
            <p style='margin-bottom: 16px;'>My oil paintings are acts of devotion — to nature, to memory, and to the fleeting moments that define life on our planet. Rooted in a deep commitment to environmental conservation and an abiding love for the remarkable ecosystems of South Florida, my work seeks to tell a story. Each painting visualizes the light and color of what came before and the quiet anticipation of what comes next, inviting the viewer to step inside a living, breathing world. My goal is to show that art and conservation become one and the same.</p>
            </div>"
        },
        new PageContentSeed
        {
            Address = "about",
            Title = "About Gloria Gronowicz",
            Content = @"<div style='place-self: center; margin-bottom: 24px;'><img src='/Other/AboutPagePhoto.JPG' alt='Gloria Gronowicz' style='max-width: 300px; width: 100%; border-radius: 8px;' /></div>
            <div style='text-align: left; max-width: 800px; margin: 0 auto;'>
            <p style='margin-bottom: 16px;'>As my early life was filled with city streets and art museums in which I played, observed and admired, I demonstrated a strong aptitude for drawing and painting. I received annual recognition for my artwork and earned the only Art Award upon graduating high school. Growing up as the child of immigrants in New York City, my father encouraged me to pursue a career that would provide more financial stability instead of the competitive world of art in a big city. I went on to earn a Ph.D. in biology from Columbia University and built a career as a scientist, educator, and ultimately an Emeritus Professor at the University of Connecticut.</p>
            <p style='margin-bottom: 16px;'>Visual analysis played a central role in my scientific work. I produced my own images for scientific publications, and authored a book on personalized medicine, for which I created nearly all the illustrations. I published many scientific, peer-reviewed articles on my research. Following my retirement from science, I have been able to fully dedicate myself to my lifelong passion for art that tells a story of life on our planet.</p>
            <p style='margin-bottom: 16px;'>My paintings are inspired by a deep love of nature and a strong commitment to environmental conservation. I am actively involved in conservation efforts in South Florida. I consider it a privilege to live in such a beautiful and unique natural setting. These values strongly influence my artistic subject matter. My oil paintings emphasize color and light to capture the beauty of the natural world. Most of my paintings tell a story of the events leading to that captured moment, and what will happen next.</p>
            </div>"
        },
        new PageContentSeed
        {
            Address = "galleries",
            Title = "Galleries",
            Content = @"<div style='text-align: left; max-width: 800px; margin: 0 auto;'>
            <p style='margin-bottom: 16px;'><strong>Emerging Artist at Stuart Artfest 2024</strong></p>
            <p style='margin-bottom: 16px;'><strong>Lighthouse Art Center</strong><br/>Annual Art Show<br/>373 Tequesta Drive<br/>Tequesta, FL 3349</p>
            <p style='margin-bottom: 16px;'><strong>Palm City Art Association</strong><br/>Annual Art Show at Cumming Library<br/>2551 SW Matheson Avenue<br/>Palm City FL. 34990</p>
            <p style='margin-bottom: 16px;'><strong>Cleveland Clinic Health and Wellness Center</strong><br/>3066 SW Martin Downs Blvd.<br/>Palm City, FL 34990</p>
            <p style='margin-bottom: 16px;'><strong>Martin Arts</strong><br/>Annual Art Show<br/>80 SE Ocean Blvd<br/>Stuart, FL 34994</p>
            <p style='margin-bottom: 16px;'><strong>Hammock Creek Golf Club</strong><br/>2400 SW Golden Bear Way<br/>Palm City, FL 34990</p>
            </div>"
        },
        new PageContentSeed
        {
            Address = "contact",
            Title = "Contact",
            Content = @"<div style='text-align: center; max-width: 800px; margin: 0 auto;'>
            <p style='margin-bottom: 16px;'>To inquire about purchasing a painting or ordering prints, please contact Gloria Gronowicz.</p>
            <p style='margin-bottom: 16px;'>Email inquiries are preferred for detailed questions about specific artworks.</p>
            <p style='margin-bottom: 16px;'><strong>Email:</strong> gloriagronowicz@gmail.com</p>
            <p style='margin-bottom: 16px;'><strong>Phone:</strong> (860) 670-0799</p>
            <p style='margin-bottom: 16px;'>I look forward to hearing from you and discussing how my art can bring beauty to your home or office.</p>
            </div>"
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
}
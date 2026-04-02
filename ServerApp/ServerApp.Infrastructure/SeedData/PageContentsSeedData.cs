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
            Content = "My paintings are what I love and believe I can capture on the canvas for that brief moment. My desire is to preserve the great beauty of our natural world through art and conservation work. </br>Gloria"
        },
        new PageContentSeed
        {
            Address = "about",
            Title = "About Gloria Gronowicz",
            Content = @"<div style='place-self: center; margin-bottom: 24px;'><img src='/Other/AboutPagePhoto.JPG' alt='Gloria Gronowicz' style='max-width: 300px; width: 100%; border-radius: 8px;' /></div>
            <div style='text-align: left; max-width: 800px; margin: 0 auto;'>
            <p style='margin-bottom: 16px;'>My early life was filled with city streets and art museums in which I played, observed and admired. I never had any formal art training or attended art school. However, at an early age, I demonstrated a strong aptitude for drawing and painting. I received annual recognition for my artwork and earned the only Art Award upon graduating high school.</p>
            <p style='margin-bottom: 16px;'>Growing up as the child of immigrants in New York City, my father—a full-time writer—encouraged me to pursue a career that would provide more financial stability instead of the competitive world of art in a big city. My mother worked full-time in the book and magazine business world and taught me hard work and perseverance. She hid from me her early life of dress and costume making for various famous venues in the city. As a child, I was surprised by her amazing figure drawing that I would beg her to show me.</p>
            <p style='margin-bottom: 16px;'>I went on to earn a Ph.D. in biology from Columbia University and built a career as a scientist, educator, and ultimately an emeritus professor at the University of Connecticut.</p>
            <p style='margin-bottom: 16px;'>Visual analysis played a central role in my scientific work, particularly through light, scanning, and electron microscopy. I later served as Director of a Histology Facility, producing images for scientific publications, and authored a book on personalized medicine, for which I created nearly all the illustrations. I published many scientific, peer-reviewed articles on my research. I wrote them as a story of discovery and proof. Following my retirement from science, I have been able to fully dedicate myself to my lifelong passion for art that tells a story of life on our planet.</p>
            <p style='margin-bottom: 16px;'>My paintings are inspired by a deep love of nature and a strong commitment to environmental conservation. I am actively involved in conservation efforts in South Florida. I consider it a privilege to live in such a beautiful and unique natural setting. These values strongly influence my artistic subject matter. I also have a particular affinity for turtles since growing up in apartment, I only had pet turtles.</p>
            <p>My art is oil painting that emphasizes color and light to capture the beauty of the natural world. Most of my paintings tell a story of the events leading to that moment in the photo, and what will happen next.</p>
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
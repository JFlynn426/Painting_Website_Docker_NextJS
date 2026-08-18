using ServerApp.Infrastructure.SeedData;

namespace ServerApp.Infrastructure.SeedData.SiteSpecific.Flynn;

/// <summary>
/// Seed data for the PageContents table for the Flynn (flynnart.com) site.
/// Duplicates the shared <see cref="PageContentsSeedData"/> entries, with a
/// Flynn-specific home page.
/// 
/// Table Schema:
/// - Id (uniqueidentifier, PK, ValueGeneratedOnAdd)
/// - Address (nvarchar(200), Required, Unique Index)
/// - Title (nvarchar(200), Required)
/// - Content (nvarchar(max), Required)
/// </summary>
public static class FlynnPageContentsSeedData
{
    public static readonly List<PageContentSeed> PageContents = new()
    {
        new PageContentSeed
        {
            Address = "home",
            Title = null,
            Content = @"<p class='pb-4'>Terri Gray lives in Jensen Beach, Florida. Terri has spent the last 40 years as a working artist and teacher and has earned a Bachelor of Arts degree in Art Education from Florida Atlantic University, Boca Raton, FL. Terri put herself through college working as a graphic artist, and continued in the field after graduating. During this time she enjoyed learning and developing new mediums and artistic techniques. Her artwork is strongly influenced by her background as a graphic artist and interprets shapes, imagery, and textures derived from the natural world.</p><p class='pb-4'>Terri shows and sells her work at Austin Pottery Studio And Gallery. Her work has been shown in a national exhibit at Charlie Cummings Gallery in Gainesville Florida. In 2023, Terri was invited to show her work at the George Washington Carver Museum in Austin, Texas. Terri has been a featured artist for the Clay Collective of Austin and the Texas Clay Arts Association.</p>",
            PhotoUrls = new[]
            {
                "/Seascapes-Thumbnail/Wind_and_Water_.jpg",
                "/Animals-Thumbnail/Hawksbill_Turtle_.jpg",
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
            Content = @"<p class='pb-4'>As my early life was filled with city streets and art museums in which I played, observed and admired, I demonstrated a strong aptitude for drawing and painting. I received annual recognition for my artwork and earned the only Art Award upon graduating high school. Growing up as the child of immigrants in New York City, my father encouraged me to pursue a career that would provide more financial stability instead of the competitive world of art in a big city. I went on to earn a Ph.D. in biology from Columbia University and built a career as a scientist, educator, and ultimately an Emeritus Professor at the University of Connecticut.</p><p class='pb-4'>Visual analysis played a central role in my scientific work. I produced my own images for scientific publications, and authored a book on personalized medicine, for which I created nearly all the illustrations. I published many scientific, peer-reviewed articles on my research. Following my retirement from science, I have been able to fully dedicate myself to my lifelong passion for art that tells a story of life on our planet.</p><p class='pb-4'>My paintings are inspired by a deep love of nature and a strong commitment to environmental conservation. I am actively involved in conservation efforts in South Florida. I consider it a privilege to live in such a beautiful and unique natural setting. These values strongly influence my artistic subject matter. My oil paintings emphasize color and light to capture the beauty of the natural world. Most of my paintings tell a story of the events leading to that captured moment, and what will happen next.</p>",
            PhotoUrls = new[] { "/Other/AboutPagePhoto.JPG" }
        },
        new PageContentSeed
        {
            Address = "galleries",
            Title = "Galleries",
            Content = @"<p class='pb-4' style='text-align: center'><strong>Emerging Artist at Stuart Artfest 2024</strong></p><p class='pb-4' style='text-align: center'><strong>Lighthouse Art Center</strong><br>Annual Art Show<br>373 Tequesta Drive<br>Tequesta, FL 3349</p><p class='pb-4' style='text-align: center'><strong>Palm City Art Association</strong><br>Annual Art Show at Cumming Library<br>2551 SW Matheson Avenue<br>Palm City FL. 34990</p><p class='pb-4' style='text-align: center'><strong>Cleveland Clinic Health and Wellness Center</strong><br>3066 SW Martin Downs Blvd.<br>Palm City, FL 34990</p><p class='pb-4' style='text-align: center'><strong>Martin Arts</strong><br>Annual Art Show<br>80 SE Ocean Blvd<br>Stuart, FL 33494</p><p class='pb-4' style='text-align: center'><strong>Hammock Creek Golf Club</strong><br>2400 SW Golden Bear Way<br>Palm City, FL 34990</p>"
        },
        new PageContentSeed
        {
            Address = "contact",
            Title = "Contact",
            Content = @"<p class='pb-4' style='text-align: center'>To inquire about purchasing a painting or ordering prints, please contact Gloria Gronowicz.</p><p class='pb-4' style='text-align: center'>Email inquiries are preferred for detailed questions about specific artworks.</p><p class='pb-4' style='text-align: center'><strong>Email:</strong> gloriagronowicz@gmail.com</p><p class='pb-4' style='text-align: center'><strong>Phone:</strong> (860) 670-0799</p><p class='pb-4' style='text-align: center'>I look forward to hearing from you and discussing how my art can bring beauty to your home or office.</p>"
        }
    };
}

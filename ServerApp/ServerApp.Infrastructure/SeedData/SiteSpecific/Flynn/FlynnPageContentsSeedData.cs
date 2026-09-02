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
            Title = "Oil Painter & Ceramic Artist | Jensen Beach, Florida",
            Content = @"<p class='pb-4'>I am a Jensen Beach, Florida–based artist and my creative practice moves fluidly between oil painting and ceramics. My work is rooted in a deep appreciation for color, texture, form, and the expressive qualities of handmade objects.</p><p class='pb-4'>Through oil painting, I explore atmosphere, emotion, and the subtle relationships between light and color. My paintings invite viewers to slow down and discover the layers, gestures, and visual rhythms of nature within each composition. In ceramics, I bring that same sensitivity to surface and form, creating pieces that celebrate the tactile nature of clay and the individuality of the handmade process. Each of my ceramic pieces are hand carved using the Sgraffito technique. I view every ceramic work as a functional canvas that can enhance everyday life experiences.</p><p class='pb-4'>Working across two distinct yet complementary mediums allows me to explore both the painted surface and the physical object. My paintings and ceramics reflect an intuitive, process-driven approach in which experimentation and discovery are central to the work.</p><p class='pb-4'>Based on Florida's Treasure Coast, which is known for its vibrant and diverse arts community, I draw inspiration from the natural environment, everyday experiences, and the ever-changing qualities of light, color, and texture. Whether working on canvas or shaping clay, I approach each piece as an opportunity to explore beauty, movement, and personal expression.</p><p class='pb-4'>My work reflects a contemporary artistic practice grounded in craftsmanship, curiosity, and a genuine connection to the creative process. My paintings and ceramics offer viewers an intimate experience of color, texture, surface, and form—each piece carrying the character and energy of the artist's hand.</p>",
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

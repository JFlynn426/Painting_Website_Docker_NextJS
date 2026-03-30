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
            Title = "Welcome to My Art Gallery",
            Content = "Explore a curated collection of original paintings featuring landscapes, seascapes, animals, and flowers. Each piece is created with passion and attention to detail, capturing the beauty of the natural world."
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
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
namespace ServerApp.Infrastructure.SeedData.SiteSpecific;

/// <summary>
/// Provides site-specific seed data for database initialization.
/// Each site implements this interface to supply its own categories, paintings, and page content.
/// </summary>
public interface ISiteSeedDataProvider
{
    /// <summary>
    /// Gets the painting categories for this site.
    /// </summary>
    IEnumerable<PaintingCategorySeed> Categories { get; }

    /// <summary>
    /// Gets the paintings for this site.
    /// </summary>
    IEnumerable<PaintingSeed> Paintings { get; }

    /// <summary>
    /// Gets the page content for this site.
    /// </summary>
    IEnumerable<PageContentSeed> PageContents { get; }
}

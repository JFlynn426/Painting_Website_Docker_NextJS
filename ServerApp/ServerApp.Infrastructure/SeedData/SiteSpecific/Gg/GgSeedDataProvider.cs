using ServerApp.Infrastructure.SeedData;

namespace ServerApp.Infrastructure.SeedData.SiteSpecific.Gg;

/// <summary>
/// Provides seed data for the GG (ggpaintings.com) site.
/// References the existing shared seed data classes.
/// </summary>
public sealed class GgSeedDataProvider : ISiteSeedDataProvider
{
    /// <inheritdoc />
    public IEnumerable<PaintingCategorySeed> Categories => PaintingCategoriesSeedData.Categories;

    /// <inheritdoc />
    public IEnumerable<PaintingSeed> Paintings => PaintingsSeedData.Paintings;

    /// <inheritdoc />
    public IEnumerable<PageContentSeed> PageContents => PageContentsSeedData.PageContents;
}

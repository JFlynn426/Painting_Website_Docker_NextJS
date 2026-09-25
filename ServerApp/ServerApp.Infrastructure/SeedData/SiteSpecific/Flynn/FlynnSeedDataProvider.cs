using ServerApp.Infrastructure.SeedData;

namespace ServerApp.Infrastructure.SeedData.SiteSpecific.Flynn;

/// <summary>
/// Provides seed data for the Flynn (flynnart.com) site.
/// Uses Flynn-specific categories, paintings, and page content.
/// </summary>
public sealed class FlynnSeedDataProvider : ISiteSeedDataProvider
{
    /// <inheritdoc />
    public IEnumerable<PaintingCategorySeed> Categories => FlynnPaintingCategoriesSeedData.Categories;

    /// <inheritdoc />
    public IEnumerable<PaintingSeed> Paintings => FlynnPaintingsSeedData.Paintings;

    /// <inheritdoc />
    public IEnumerable<PageContentSeed> PageContents => FlynnPageContentsSeedData.PageContents;
}

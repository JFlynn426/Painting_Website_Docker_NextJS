using ServerApp.Infrastructure.SeedData;

namespace ServerApp.Infrastructure.SeedData.SiteSpecific.Flynn;

/// <summary>
/// Provides seed data for the Flynn (flynnart.com) site.
/// Uses Flynn-specific page content; categories and paintings are shared with GG.
/// </summary>
public sealed class FlynnSeedDataProvider : ISiteSeedDataProvider
{
    /// <inheritdoc />
    public IEnumerable<PaintingCategorySeed> Categories => PaintingCategoriesSeedData.Categories;

    /// <inheritdoc />
    public IEnumerable<PaintingSeed> Paintings => PaintingsSeedData.Paintings;

    /// <inheritdoc />
    public IEnumerable<PageContentSeed> PageContents => FlynnPageContentsSeedData.PageContents;
}

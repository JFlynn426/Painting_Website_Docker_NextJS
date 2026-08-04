using ServerApp.Infrastructure.SeedData;

namespace ServerApp.Infrastructure.SeedData.SiteSpecific.Flynn;

/// <summary>
/// Provides seed data for the Flynn (flynnart.com) site.
/// Currently references the same seed data as GG for initial deployment.
/// </summary>
public sealed class FlynnSeedDataProvider : ISiteSeedDataProvider
{
    /// <inheritdoc />
    public IEnumerable<PaintingCategorySeed> Categories => PaintingCategoriesSeedData.Categories;

    /// <inheritdoc />
    public IEnumerable<PaintingSeed> Paintings => PaintingsSeedData.Paintings;

    /// <inheritdoc />
    public IEnumerable<PageContentSeed> PageContents => PageContentsSeedData.PageContents;
}

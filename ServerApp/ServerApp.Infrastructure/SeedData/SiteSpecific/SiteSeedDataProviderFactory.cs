using ServerApp.Infrastructure.SeedData.SiteSpecific.Flynn;
using ServerApp.Infrastructure.SeedData.SiteSpecific.Gg;

namespace ServerApp.Infrastructure.SeedData.SiteSpecific;

/// <summary>
/// Factory for resolving site-specific seed data providers based on the SITE_NAME environment variable.
/// </summary>
public static class SiteSeedDataProviderFactory
{
    /// <summary>
    /// Gets the seed data provider for the specified site name.
    /// </summary>
    /// <param name="siteName">The site identifier (e.g., "gg", "flynn").</param>
    /// <returns>The appropriate <see cref="ISiteSeedDataProvider"/> implementation.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="siteName"/> is null, empty, or unrecognized.</exception>
    public static ISiteSeedDataProvider GetProvider(string? siteName)
    {
        if (string.IsNullOrWhiteSpace(siteName))
        {
            throw new ArgumentException("Site name cannot be null or empty.", nameof(siteName));
        }

        return siteName.ToLowerInvariant() switch
        {
            "gg" => new GgSeedDataProvider(),
            "flynn" => new FlynnSeedDataProvider(),
            _ => throw new ArgumentException(
                $"Unknown site name: {siteName}. Supported sites: gg, flynn",
                nameof(siteName))
        };
    }
}

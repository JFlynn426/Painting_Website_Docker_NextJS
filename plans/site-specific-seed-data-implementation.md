# C# Site-Specific Seed Data Implementation Plan

**Goal:** Implement site-specific seed data using C# static classes so the GG and Flynn sites can be initialized with different categories, paintings, and page content on first deployment.

**Strategy:** Create site-specific seed data classes organized by site, modify `DatabaseSeeder` to select seed data based on the `SITE_NAME` environment variable, and update docker-compose to pass `SITE_NAME` to each API container.

**Key Insight:** Seed data only runs on empty databases. Normal deployments restore from backup, bypassing the seeder entirely. Therefore, C# static classes are ideal — no runtime file I/O, no JSON deserialization, no volume mounts needed.

---

## Current Architecture

### Seed Data Flow

```
AppInitializer.StartAsync()
  └─ DatabaseSeeder.SeedAsync()
       └─ SeedDatabaseAsync(WriteDbContext)  [skips if data exists]
            └─ PaintingCategoriesSeedData.Categories
            └─ PaintingsSeedData.Paintings
            └─ PageContentsSeedData.PageContents
       └─ SeedDatabaseAsync(ReadDbContext)   [skips if data exists]
```

### Current Seed Data Files

| File | Purpose | Records |
|------|---------|---------|
| [`PaintingCategoriesSeedData.cs`](ServerApp/ServerApp.Infrastructure/SeedData/PaintingCategoriesSeedData.cs) | 5 categories (Landscapes, Seascapes, Animals, Flowers, New Paintings) | 5 |
| [`PaintingsSeedData.cs`](ServerApp/ServerApp.Infrastructure/SeedData/PaintingsSeedData.cs) | Aggregates from 4 category files | ~100+ |
| [`SeascapesSeedData.cs`](ServerApp/ServerApp.Infrastructure/SeedData/SeascapesSeedData.cs) | Seascape paintings | ~17 |
| [`AnimalsSeedData.cs`](ServerApp/ServerApp.Infrastructure/SeedData/AnimalsSeedData.cs) | Animal paintings | ~30 |
| [`LandscapesAndCityscapesSeedData.cs`](ServerApp/ServerApp.Infrastructure/SeedData/LandscapesAndCityscapesSeedData.cs) | Landscape paintings | ~15 |
| [`FlowersSeedData.cs`](ServerApp/ServerApp.Infrastructure/SeedData/FlowersSeedData.cs) | Flower paintings | ~12 |
| [`PageContentsSeedData.cs`](ServerApp/ServerApp.Infrastructure/SeedData/PageContentsSeedData.cs) | Home, About, Galleries, Contact pages | 4 |

### Data Models (Reused)

| Class | Location | Purpose |
|-------|----------|---------|
| `PaintingSeed` | [`PaintingsSeedData.cs:13`](ServerApp/ServerApp.Infrastructure/SeedData/PaintingsSeedData.cs:13) | Painting seed data DTO |
| `PaintingCategorySeed` | [`PaintingCategoriesSeedData.cs:56`](ServerApp/ServerApp.Infrastructure/SeedData/PaintingCategoriesSeedData.cs:56) | Category seed data DTO |
| `PageContentSeed` | [`PageContentsSeedData.cs:61`](ServerApp/ServerApp.Infrastructure/SeedData/PageContentsSeedData.cs:61) | Page content seed data DTO |

---

## Implementation Plan

### Phase 1: Create Site-Specific Seed Data Directory Structure

**New Directory Structure:**

```
ServerApp/ServerApp.Infrastructure/SeedData/
  ├── GG/
  │   ├── GGCategoriesSeedData.cs
  │   ├── GGSeascapesSeedData.cs
  │   ├── GGAnimalsSeedData.cs
  │   ├── GGLandscapesAndCityscapesSeedData.cs
  │   ├── GGFlowersSeedData.cs
  │   ├── GGPaintingsSeedData.cs
  │   └── GGPageContentsSeedData.cs
  ├── Flynn/
  │   ├── FlynnCategoriesSeedData.cs
  │   ├── FlynnSeascapesSeedData.cs
  │   ├── FlynnAnimalsSeedData.cs
  │   ├── FlynnLandscapesAndCityscapesSeedData.cs
  │   ├── FlynnFlowersSeedData.cs
  │   ├── FlynnPaintingsSeedData.cs
  │   └── FlynnPageContentsSeedData.cs
  ├── (existing shared files remain unchanged)
  │   ├── PaintingCategoriesSeedData.cs
  │   ├── PaintingsSeedData.cs
  │   ├── SeascapesSeedData.cs
  │   ├── AnimalsSeedData.cs
  │   ├── LandscapesAndCityscapesSeedData.cs
  │   ├── FlowersSeedData.cs
  │   ├── PageContentsSeedData.cs
  │   └── PaintingsSeedDataOld.cs
```

**Rationale for structure:**
- Each site gets its own subdirectory with prefixed class names to avoid namespace collisions
- Category-level painting files (Seascapes, Animals, etc.) are kept separate for maintainability
- `GGPaintingsSeedData.cs` aggregates the category files (matching the pattern in [`PaintingsSeedData.cs`](ServerApp/ServerApp.Infrastructure/SeedData/PaintingsSeedData.cs))
- Existing shared files remain as fallback/default for backward compatibility

### Phase 2: Create Site-Specific Seed Data Classes

#### GG Site Classes

Each GG class will be a **static class** with a `public static readonly` collection, matching the existing pattern in [`SeascapesSeedData.cs`](ServerApp/ServerApp.Infrastructure/SeedData/SeascapesSeedData.cs).

**`GGCategoriesSeedData.cs`:**
```csharp
namespace ServerApp.Infrastructure.SeedData.GG;

/// <summary>
/// Seed data for GG site painting categories.
/// </summary>
public static class GGCategoriesSeedData
{
    public static readonly IEnumerable<PaintingCategorySeed> Categories = new[]
    {
        // Copy from PaintingCategoriesSeedData.Categories
        // Customize category names/descriptions for GG site
    };
}
```

**`GGPaintingsSeedData.cs`** (aggregator, matching [`PaintingsSeedData.cs`](ServerApp/ServerApp.Infrastructure/SeedData/PaintingsSeedData.cs:10)):
```csharp
namespace ServerApp.Infrastructure.SeedData.GG;

/// <summary>
/// Aggregates all GG site painting seed data from category-specific files.
/// </summary>
public static class GGPaintingsSeedData
{
    public static readonly IEnumerable<PaintingSeed> Seascapes = GGSeascapesSeedData.Seascapes;
    public static readonly IEnumerable<PaintingSeed> Animals = GGAnimalsSeedData.Animals;
    public static readonly IEnumerable<PaintingSeed> LandscapesAndCityscapes = GGLandscapesAndCityscapesSeedData.LandscapesAndCityscapes;
    public static readonly IEnumerable<PaintingSeed> Flowers = GGFlowersSeedData.Flowers;

    public static readonly IEnumerable<PaintingSeed> Paintings =
        Seascapes.Concat(Animals).Concat(LandscapesAndCityscapes).Concat(Flowers);
}
```

**`GGPageContentsSeedData.cs`:**
```csharp
namespace ServerApp.Infrastructure.SeedData.GG;

/// <summary>
/// Seed data for GG site page content.
/// </summary>
public static class GGPageContentsSeedData
{
    public static readonly IEnumerable<PageContentSeed> PageContents = new[]
    {
        // Copy from PageContentsSeedData.PageContents
        // Customize content for GG site (Gloria Gronowicz)
    };
}
```

#### Flynn Site Classes

Same pattern with `Flynn` prefix and `ServerApp.Infrastructure.SeedData.Flynn` namespace. Content will be customized for the Flynn/Terri Gray site.

### Phase 3: Create Seed Data Provider Interface and Implementations

**New File: `ISiteSeedDataProvider.cs`**
```csharp
namespace ServerApp.Infrastructure.SeedData;

/// <summary>
/// Provides site-specific seed data for database initialization.
/// </summary>
public interface ISiteSeedDataProvider
{
    /// <summary>
    /// Gets the site identifier (e.g., "gg", "flynn").
    /// </summary>
    string SiteName { get; }

    /// <summary>
    /// Gets the painting category seed data.
    /// </summary>
    IEnumerable<PaintingCategorySeed> Categories { get; }

    /// <summary>
    /// Gets the painting seed data.
    /// </summary>
    IEnumerable<PaintingSeed> Paintings { get; }

    /// <summary>
    /// Gets the page content seed data.
    /// </summary>
    IEnumerable<PageContentSeed> PageContents { get; }
}
```

**New File: `GgSeedDataProvider.cs`**
```csharp
using ServerApp.Infrastructure.SeedData.GG;

namespace ServerApp.Infrastructure.SeedData;

/// <summary>
/// Provides seed data for the GG (Gloria Gronowicz) site.
/// </summary>
internal sealed class GgSeedDataProvider : ISiteSeedDataProvider
{
    public string SiteName => "gg";
    public IEnumerable<PaintingCategorySeed> Categories => GGCategoriesSeedData.Categories;
    public IEnumerable<PaintingSeed> Paintings => GGPaintingsSeedData.Paintings;
    public IEnumerable<PageContentSeed> PageContents => GGPageContentsSeedData.PageContents;
}
```

**New File: `FlynnSeedDataProvider.cs`**
```csharp
using ServerApp.Infrastructure.SeedData.Flynn;

namespace ServerApp.Infrastructure.SeedData;

/// <summary>
/// Provides seed data for the Flynn (Terri Gray) site.
/// </summary>
internal sealed class FlynnSeedDataProvider : ISiteSeedDataProvider
{
    public string SiteName => "flynn";
    public IEnumerable<PaintingCategorySeed> Categories => FlynnCategoriesSeedData.Categories;
    public IEnumerable<PaintingSeed> Paintings => FlynnPaintingsSeedData.Paintings;
    public IEnumerable<PageContentSeed> PageContents => FlynnPageContentsSeedData.PageContents;
}
```

**New File: `DefaultSeedDataProvider.cs`** (fallback using existing shared data)
```csharp
namespace ServerApp.Infrastructure.SeedData;

/// <summary>
/// Provides default seed data using the shared seed data classes.
/// Used when SITE_NAME is not configured or unrecognized.
/// </summary>
internal sealed class DefaultSeedDataProvider : ISiteSeedDataProvider
{
    public string SiteName => "default";
    public IEnumerable<PaintingCategorySeed> Categories => PaintingCategoriesSeedData.Categories;
    public IEnumerable<PaintingSeed> Paintings => PaintingsSeedData.Paintings;
    public IEnumerable<PageContentSeed> PageContents => PageContentsSeedData.PageContents;
}
```

### Phase 4: Modify DatabaseSeeder to Use Site-Specific Seed Data

**File to Modify: [`DatabaseSeeder.cs`](ServerApp/ServerApp.Infrastructure/Services/DatabaseSeeder.cs)**

**Changes:**
1. Add `IConfiguration` dependency to constructor
2. Add `ISiteSeedDataProvider` property (resolved from `SITE_NAME` env var)
3. Replace direct references to `PaintingCategoriesSeedData.Categories` etc. with provider references
4. Log which site's seed data is being used

**Updated Constructor:**
```csharp
private readonly IConfiguration _configuration;
private readonly ISiteSeedDataProvider _seedDataProvider;

public DatabaseSeeder(
    ILogger<DatabaseSeeder> logger,
    IConfiguration configuration,
    IPaintingCategoryFactory categoryFactory,
    IPaintingFactory paintingFactory,
    IPageContentFactory pageContentFactory,
    WriteDbContext writeDbContext,
    ReadDbContext readDbContext)
{
    _logger = logger;
    _configuration = configuration;
    _categoryFactory = categoryFactory;
    _paintingFactory = paintingFactory;
    _pageContentFactory = pageContentFactory;
    _writeDbContext = writeDbContext;
    _readDbContext = readDbContext;
    _seedDataProvider = ResolveSeedDataProvider();
}
```

**New Private Method:**
```csharp
private ISiteSeedDataProvider ResolveSeedDataProvider()
{
    var siteName = _configuration["SITE_NAME"]?.ToLowerInvariant();

    return siteName switch
    {
        "gg" => new GgSeedDataProvider(),
        "flynn" => new FlynnSeedDataProvider(),
        _ =>
        {
            _logger.LogWarning("SITE_NAME not configured or unrecognized. Using default seed data.");
            return new DefaultSeedDataProvider();
        }
    }
}
```

**Updated `SeedDatabaseAsync`** — Replace these lines:
```csharp
// Before:
foreach (var seedCategory in PaintingCategoriesSeedData.Categories)

// After:
foreach (var seedCategory in _seedDataProvider.Categories)
```

```csharp
// Before:
foreach (var seedPainting in PaintingsSeedData.Paintings)

// After:
foreach (var seedPainting in _seedDataProvider.Paintings)
```

```csharp
// Before:
foreach (var seedPageContent in PageContentsSeedData.PageContents)

// After:
foreach (var seedPageContent in _seedDataProvider.PageContents)
```

**Add logging at start of `SeedAsync`:**
```csharp
_logger.LogInformation("Seeding database for site: {SiteName}", _seedDataProvider.SiteName);
```

### Phase 5: Add SITE_NAME to Docker Compose Configuration

**Files to Modify:**
- [`docker-compose/docker-compose.multi.yml`](docker-compose/docker-compose.multi.yml)
- [`docker-compose/docker-compose.multi.arm64.yml`](docker-compose/docker-compose.multi.arm64.yml)

**Change for `api-gg` service** (add to `environment:` block):
```yaml
SITE_NAME: gg
```

**Change for `api-flynn` service** (add to `environment:` block):
```yaml
SITE_NAME: flynn
```

### Phase 6: Update Plans to Remove JSON References

**Files to Update:**
- [`plans/multi-site-deployment-architecture.md`](plans/multi-site-deployment-architecture.md)
- [`plans/multi-site-implementation-plan.md`](plans/multi-site-implementation-plan.md)

**Changes:**
1. Replace "Externalize to JSON" strategy with "C# site-specific seed classes" in architecture table
2. Remove JSON file format sections (lines 501-643 in architecture plan)
3. Remove seed data volume mount references from docker-compose examples
4. Update implementation phases to reflect C# approach
5. Update risk assessment to remove JSON-related risks
6. Update decision summary table

---

## Files Summary

### New Files (11)

| File | Purpose |
|------|---------|
| `ServerApp/ServerApp.Infrastructure/SeedData/ISiteSeedDataProvider.cs` | Interface for site seed data |
| `ServerApp/ServerApp.Infrastructure/SeedData/GgSeedDataProvider.cs` | GG site provider |
| `ServerApp/ServerApp.Infrastructure/SeedData/FlynnSeedDataProvider.cs` | Flynn site provider |
| `ServerApp/ServerApp.Infrastructure/SeedData/DefaultSeedDataProvider.cs` | Default/fallback provider |
| `ServerApp/ServerApp.Infrastructure/SeedData/GG/GGCategoriesSeedData.cs` | GG categories |
| `ServerApp/ServerApp.Infrastructure/SeedData/GG/GGSeascapesSeedData.cs` | GG seascapes |
| `ServerApp/ServerApp.Infrastructure/SeedData/GG/GGAnimalsSeedData.cs` | GG animals |
| `ServerApp/ServerApp.Infrastructure/SeedData/GG/GGLandscapesAndCityscapesSeedData.cs` | GG landscapes |
| `ServerApp/ServerApp.Infrastructure/SeedData/GG/GGFlowersSeedData.cs` | GG flowers |
| `ServerApp/ServerApp.Infrastructure/SeedData/GG/GGPaintingsSeedData.cs` | GG paintings aggregator |
| `ServerApp/ServerApp.Infrastructure/SeedData/GG/GGPageContentsSeedData.cs` | GG page content |

### Flynn Site Files (7) — Same Pattern

| File | Purpose |
|------|---------|
| `ServerApp/ServerApp.Infrastructure/SeedData/Flynn/FlynnCategoriesSeedData.cs` | Flynn categories |
| `ServerApp/ServerApp.Infrastructure/SeedData/Flynn/FlynnSeascapesSeedData.cs` | Flynn seascapes |
| `ServerApp/ServerApp.Infrastructure/SeedData/Flynn/FlynnAnimalsSeedData.cs` | Flynn animals |
| `ServerApp/ServerApp.Infrastructure/SeedData/Flynn/FlynnLandscapesAndCityscapesSeedData.cs` | Flynn landscapes |
| `ServerApp/ServerApp.Infrastructure/SeedData/Flynn/FlynnFlowersSeedData.cs` | Flynn flowers |
| `ServerApp/ServerApp.Infrastructure/SeedData/Flynn/FlynnPaintingsSeedData.cs` | Flynn paintings aggregator |
| `ServerApp/ServerApp.Infrastructure/SeedData/Flynn/FlynnPageContentsSeedData.cs` | Flynn page content |

### Modified Files (5)

| File | Change |
|------|--------|
| [`DatabaseSeeder.cs`](ServerApp/ServerApp.Infrastructure/Services/DatabaseSeeder.cs) | Add `IConfiguration`, use `ISiteSeedDataProvider` |
| [`docker-compose.multi.yml`](docker-compose/docker-compose.multi.yml) | Add `SITE_NAME` env var to api-gg and api-flynn |
| [`docker-compose.multi.arm64.yml`](docker-compose/docker-compose.multi.arm64.yml) | Add `SITE_NAME` env var to api-gg and api-flynn |
| [`plans/multi-site-deployment-architecture.md`](plans/multi-site-deployment-architecture.md) | Replace JSON strategy with C# approach |
| [`plans/multi-site-implementation-plan.md`](plans/multi-site-implementation-plan.md) | Replace JSON strategy with C# approach |

### Unchanged Files

| File | Reason |
|------|--------|
| `PaintingSeed` class | Reused as-is by all seed data classes |
| `PaintingCategorySeed` class | Reused as-is by all seed data classes |
| `PageContentSeed` class | Reused as-is by all seed data classes |
| Existing shared seed data files | Remain as default fallback |
| `AppInitializer.cs` | No changes needed |
| `ServerApp.Api/Dockerfile` | No changes needed |
| `InfrastructureExtensions.cs` | No changes needed (seeder already registered) |

---

## Coding Standards Applied

| Standard | Application |
|----------|-------------|
| **`using` directives** | All files use explicit `using` statements per [`AGENTS.md`](.roo/rules-code/AGENTS.md) rules |
| **`internal sealed`** | Provider classes are `internal sealed` (not part of public API) |
| **`public static`** | Seed data classes use `public static` with `readonly` collections (existing pattern) |
| **XML doc comments** | All new classes and interfaces include XML documentation |
| **Namespace organization** | Site-specific classes use sub-namespaces (`ServerApp.Infrastructure.SeedData.GG`, `ServerApp.Infrastructure.SeedData.Flynn`) |
| **Interface segregation** | `ISiteSeedDataProvider` provides clean abstraction for site-specific data |
| **Pattern matching** | `ResolveSeedDataProvider()` uses C# `switch` expression for clean site resolution |
| **Defensive logging** | Logs site name at seed start, warns on unrecognized `SITE_NAME` |
| **Null safety** | `_configuration["SITE_NAME"]?.ToLowerInvariant()` with null-conditional |
| **CancellationToken** | Preserved throughout seeding flow (existing pattern) |

---

## Testing Strategy

1. **Unit test `ResolveSeedDataProvider()`** — Verify correct provider returned for "gg", "flynn", null, and unknown values
2. **Integration test** — Start API container with `SITE_NAME=gg`, verify GG seed data loaded
3. **Integration test** — Start API container with `SITE_NAME=flynn`, verify Flynn seed data loaded
4. **Fallback test** — Start API container without `SITE_NAME`, verify default seed data loaded with warning log

---

## Rollback Plan

If issues arise:
1. Remove `SITE_NAME` env var from docker-compose — falls back to default (existing) seed data
2. Revert `DatabaseSeeder.cs` to use `PaintingCategoriesSeedData.Categories` directly
3. Delete new seed data directories

The existing seed data files remain untouched and serve as the fallback, ensuring zero risk of data loss.

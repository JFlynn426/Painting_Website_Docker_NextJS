namespace ServerApp.Domain.Factories;

using ServerApp.Domain.Entities;
using ServerApp.Domain.ValueObjects.Painting;
using ServerApp.Domain.ValueObjects.PaintingCategory;

public class PaintingFactory : IPaintingFactory
{
    public Painting Create(
        PaintingName title,
        PaintingDescription? description,
        PaintingImageUrl imageUrl,
        PaintingThumbnailUrl? thumbnailUrl,
        PaintingCategorySlug categorySlug,
        PaintingPrice? price,
        PaintingWidth? width = null,
        PaintingHeight? height = null,
        PaintingDepth? depth = null,
        PaintingYear? year = null,
        PaintingIsAvailable isAvailable = default!,
        PaintingIsNew isNew = default!,
        PaintingIsLandscape isLandscape = default!,
        PaintingSlug? slug = null)
    {
        // Auto-generate ID (single source of truth for ID generation)
        var id = new PaintingID();

        // Use the provided slug, or auto-generate from title (single source of truth for slug generation)
        var finalSlug = slug ?? PaintingSlug.FromTitle(title);

        var painting = new Painting(id, title, finalSlug, description, imageUrl, thumbnailUrl, categorySlug, price, width, height, depth, year, isAvailable, isNew, isLandscape);
        return painting;
    }
}
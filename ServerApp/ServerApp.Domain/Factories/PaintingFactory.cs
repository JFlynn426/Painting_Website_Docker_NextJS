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
        PaintingIsNew isNew = default!)
    {
        // Auto-generate ID (single source of truth for ID generation)
        var id = new PaintingID();

        // Auto-generate slug from title (single source of truth for slug generation)
        var slug = PaintingSlug.FromTitle(title);

        var painting = new Painting(id, title, slug, description, imageUrl, thumbnailUrl, categorySlug, price, width, height, depth, year, isAvailable, isNew);
        return painting;
    }
}
namespace ServerApp.Domain.Factories;

using ServerApp.Domain.Entities;
using ServerApp.Domain.ValueObjects.Painting;
using ServerApp.Domain.ValueObjects.PaintingCategory;

public class PaintingFactory : IPaintingFactory
{
    public Painting Create(
        PaintingID id,
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
        PaintingIsAvailable isAvailable = default!)
    {
        var painting = new Painting(id, title, description, imageUrl, thumbnailUrl, categorySlug, price, width, height, depth, year, isAvailable);
        return painting;
    }
}
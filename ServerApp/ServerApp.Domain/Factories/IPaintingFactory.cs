namespace ServerApp.Domain.Factories;

using ServerApp.Domain.Entities;
using ServerApp.Domain.ValueObjects.Painting;
using ServerApp.Domain.ValueObjects.PaintingCategory;

public interface IPaintingFactory
{
    Task<Painting> CreateAsync(
        PaintingID id,
        PaintingName title,
        PaintingDescription description,
        PaintingImageUrl imageUrl,
        PaintingThumbnailUrl thumbnailUrl,
        PaintingCategorySlug categorySlug,
        PaintingPrice? price,
        PaintingWidth? width = null,
        PaintingHeight? height = null,
        PaintingDepth? depth = null,
        PaintingYear? year = null,
        PaintingIsAvailable isAvailable = default!,
        CancellationToken cancellationToken = default);
}
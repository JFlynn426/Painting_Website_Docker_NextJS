namespace ServerApp.Domain.Factories;

using ServerApp.Domain.Entities;
using ServerApp.Domain.Exceptions;
using ServerApp.Domain.Repositories;
using ServerApp.Domain.ValueObjects.Painting;
using ServerApp.Domain.ValueObjects.PaintingCategory;

public class PaintingFactory : IPaintingFactory
{
    private readonly IPaintingCategoryRepository _categoryRepository;

    public PaintingFactory(IPaintingCategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Painting> CreateAsync(
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
        CancellationToken cancellationToken = default)
    {
        // Validate that the category exists
        var category = await _categoryRepository.FindBySlugAsync(categorySlug, cancellationToken);
        if (category == null)
        {
            throw new PaintingMustHaveAnAssignedCategoryException();
        }

        var painting = new Painting(id, title, description, imageUrl, thumbnailUrl, categorySlug, price, width, height, depth, year, isAvailable);
        painting.AssignCategory(category);

        return painting;
    }
}
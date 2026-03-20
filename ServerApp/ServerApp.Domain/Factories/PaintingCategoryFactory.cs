namespace ServerApp.Domain.Factories;

using ServerApp.Domain.Entities;
using ServerApp.Domain.Exceptions;
using ServerApp.Domain.Repositories;
using ServerApp.Domain.ValueObjects.PaintingCategory;

public class PaintingCategoryFactory : IPaintingCategoryFactory
{
    private readonly IPaintingCategoryRepository _repository;

    public PaintingCategoryFactory(IPaintingCategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaintingCategory> CreateAsync(
        PaintingCategoryID id,
        PaintingCategoryName name,
        PaintingCategoryDescription? description,
        CancellationToken cancellationToken = default)
    {
        // Check if a category with this name already exists
        bool exists = await _repository.ExistsByNameAsync(name, cancellationToken);
        if (exists)
        {
            throw new PaintingCategoryNameAlreadyExistsException(name.Value);
        }

        // Auto-generate slug from name
        var slug = PaintingCategorySlug.FromName(name);

        return new PaintingCategory(id, name, slug, description);
    }
}
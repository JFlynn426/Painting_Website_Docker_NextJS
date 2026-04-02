namespace ServerApp.Domain.Factories;

using ServerApp.Domain.Entities;
using ServerApp.Domain.Exceptions;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Domain.ValueObjects.PaintingCategory;

public class PaintingCategoryFactory : IPaintingCategoryFactory
{
    private readonly IPaintingCategoryReadRepository _readRepository;

    public PaintingCategoryFactory(IPaintingCategoryReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<PaintingCategory> CreateAsync(
        string name,
        string? description,
        CancellationToken cancellationToken = default)
    {
        // Auto-generate ID (single source of truth for ID generation)
        var id = new PaintingCategoryID();

        // Check if a category with this name already exists
        bool exists = await _readRepository.ExistsByNameAsync(new PaintingCategoryName(name), cancellationToken);
        if (exists)
        {
            throw new PaintingCategoryNameAlreadyExistsException(name);
        }

        // Auto-generate slug from name
        var slug = PaintingCategorySlug.FromName(new PaintingCategoryName(name));

        return new PaintingCategory(id, new PaintingCategoryName(name), slug, description);
    }
}
namespace ServerApp.Domain.Repositories.Read;

using ServerApp.Domain.Entities;
using ServerApp.Domain.ValueObjects.Painting;
using ServerApp.Domain.ValueObjects.PaintingCategory;

/// <summary>
/// Read repository interface for Painting entities.
/// Handles only read operations (Get, GetAll, GetByCategory, Exists).
/// </summary>
public interface IPaintingReadRepository
{
    Task<Painting?> GetAsync(PaintingSlug slug, CancellationToken cancellationToken = default);
    Task<IEnumerable<Painting>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Painting>> GetByCategoryAsync(PaintingCategorySlug categorySlug, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySlugAsync(PaintingSlug slug, CancellationToken cancellationToken = default);
}
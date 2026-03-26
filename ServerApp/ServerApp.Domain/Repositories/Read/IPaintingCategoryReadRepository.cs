namespace ServerApp.Domain.Repositories.Read;

using ServerApp.Domain.Entities;
using ServerApp.Domain.ValueObjects.PaintingCategory;

/// <summary>
/// Read repository interface for PaintingCategory entities.
/// Handles only read operations (Get, FindBySlug, GetAll, Exists).
/// </summary>
public interface IPaintingCategoryReadRepository
{
    Task<PaintingCategory?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaintingCategory?> FindBySlugAsync(PaintingCategorySlug slug, CancellationToken cancellationToken = default);
    Task<IEnumerable<PaintingCategory>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(PaintingCategoryName name, CancellationToken cancellationToken = default);
}
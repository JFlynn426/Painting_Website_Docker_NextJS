namespace ServerApp.Domain.Repositories;

using ServerApp.Domain.Entities;
using ServerApp.Domain.ValueObjects.Painting;
using ServerApp.Domain.ValueObjects.PaintingCategory;

public interface IPaintingRepository
{
    Task<Painting?> GetAsync(PaintingSlug slug, CancellationToken cancellationToken = default);
    Task<IEnumerable<Painting>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Painting>> GetByCategoryAsync(PaintingCategorySlug categorySlug, CancellationToken cancellationToken = default);
    Task AddAsync(Painting painting, CancellationToken cancellationToken = default);
    Task UpdateAsync(Painting painting, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySlugAsync(PaintingSlug slug, CancellationToken cancellationToken = default);
}
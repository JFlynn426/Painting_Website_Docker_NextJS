namespace ServerApp.Domain.Repositories.Write;

using ServerApp.Domain.Entities;
using ServerApp.Domain.ValueObjects.PaintingCategory;

/// <summary>
/// Write repository interface for Painting entities.
/// Handles only write operations (Add, Update, Delete).
/// </summary>
public interface IPaintingWriteRepository
{
    Task<Painting?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Painting>> GetByCategoryAsync(PaintingCategorySlug categorySlug, CancellationToken cancellationToken = default);
    Task AddAsync(Painting painting, CancellationToken cancellationToken = default);
    Task UpdateAsync(Painting painting, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
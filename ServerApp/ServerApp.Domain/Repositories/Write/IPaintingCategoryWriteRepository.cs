namespace ServerApp.Domain.Repositories.Write;

using ServerApp.Domain.Entities;

/// <summary>
/// Write repository interface for PaintingCategory entities.
/// Handles only write operations (Add, Update, Delete).
/// </summary>
public interface IPaintingCategoryWriteRepository
{
    Task AddAsync(PaintingCategory category, CancellationToken cancellationToken = default);
    Task UpdateAsync(PaintingCategory category, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
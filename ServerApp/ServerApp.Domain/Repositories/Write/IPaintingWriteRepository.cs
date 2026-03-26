namespace ServerApp.Domain.Repositories.Write;

using ServerApp.Domain.Entities;

/// <summary>
/// Write repository interface for Painting entities.
/// Handles only write operations (Add, Update, Delete).
/// </summary>
public interface IPaintingWriteRepository
{
    Task AddAsync(Painting painting, CancellationToken cancellationToken = default);
    Task UpdateAsync(Painting painting, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
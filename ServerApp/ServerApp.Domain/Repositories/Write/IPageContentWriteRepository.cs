namespace ServerApp.Domain.Repositories.Write;

using ServerApp.Domain.Entities;

/// <summary>
/// Write repository interface for PageContent entities.
/// Handles only write operations (Add, Update, Delete).
/// </summary>
public interface IPageContentWriteRepository
{
    Task<PageContent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(PageContent pageContent, CancellationToken cancellationToken = default);
    Task UpdateAsync(PageContent pageContent, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
namespace ServerApp.Domain.Repositories.Read;

using ServerApp.Domain.Entities;
using ServerApp.Domain.ValueObjects.Page;

/// <summary>
/// Read repository interface for PageContent entities.
/// Handles only read operations (Get, Exists).
/// </summary>
public interface IPageContentReadRepository
{
    Task<PageContent?> GetByAddressAsync(PageAddress address, CancellationToken cancellationToken = default);
    Task<bool> ExistsByAddressAsync(PageAddress address, CancellationToken cancellationToken = default);
}
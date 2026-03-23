namespace ServerApp.Domain.Repositories;

using ServerApp.Domain.Entities;
using ServerApp.Domain.ValueObjects.Page;

public interface IPageContentRepository
{
    Task<PageContent?> GetByAddressAsync(PageAddress address, CancellationToken cancellationToken = default);
    Task AddAsync(PageContent pageContent, CancellationToken cancellationToken = default);
    Task UpdateAsync(PageContent pageContent, CancellationToken cancellationToken = default);
    Task<bool> ExistsByAddressAsync(PageAddress address, CancellationToken cancellationToken = default);
}
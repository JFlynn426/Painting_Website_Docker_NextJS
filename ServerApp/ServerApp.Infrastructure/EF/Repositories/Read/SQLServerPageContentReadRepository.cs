namespace ServerApp.Infrastructure.EF.Repositories.Read;

using Microsoft.EntityFrameworkCore;
using ServerApp.Domain.Entities;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Domain.ValueObjects.Page;
using ServerApp.Infrastructure.EF.Contexts;

/// <summary>
/// SQL Server implementation of IPageContentReadRepository.
/// Handles only read operations using ReadDbContext.
/// </summary>
internal class SQLServerPageContentReadRepository : IPageContentReadRepository
{
    private readonly ReadDbContext _readContext;

    public SQLServerPageContentReadRepository(ReadDbContext readContext)
    {
        _readContext = readContext;
    }

    public async Task<PageContent?> GetByAddressAsync(PageAddress address, CancellationToken cancellationToken = default)
    {
        return await _readContext.PageContents
            .FirstOrDefaultAsync(p => p.Address == address.Value, cancellationToken);
    }

    public async Task<bool> ExistsByAddressAsync(PageAddress address, CancellationToken cancellationToken = default)
    {
        return await _readContext.PageContents.AnyAsync(p => p.Address == address.Value, cancellationToken);
    }
}
namespace ServerApp.Infrastructure.EF.Repositories.Write;

using Microsoft.EntityFrameworkCore;
using ServerApp.Domain.Entities;
using ServerApp.Domain.Repositories.Write;
using ServerApp.Infrastructure.EF.Contexts;

/// <summary>
/// SQL Server implementation of IPageContentWriteRepository.
/// Handles only write operations using WriteDbContext.
/// </summary>
internal class SQLServerPageContentWriteRepository : IPageContentWriteRepository
{
    private readonly WriteDbContext _writeContext;

    public SQLServerPageContentWriteRepository(WriteDbContext writeContext)
    {
        _writeContext = writeContext;
    }

    public async Task AddAsync(PageContent pageContent, CancellationToken cancellationToken = default)
    {
        await _writeContext.PageContents.AddAsync(pageContent, cancellationToken);
    }

    public async Task UpdateAsync(PageContent pageContent, CancellationToken cancellationToken = default)
    {
        _writeContext.PageContents.Update(pageContent);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pageContent = await _writeContext.PageContents.FindAsync(new object[] { id }, cancellationToken);
        if (pageContent != null)
        {
            _writeContext.PageContents.Remove(pageContent);
        }
    }
}
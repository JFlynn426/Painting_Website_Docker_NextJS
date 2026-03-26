namespace ServerApp.Infrastructure.EF.Repositories.Write;

using Microsoft.EntityFrameworkCore;
using ServerApp.Domain.Entities;
using ServerApp.Domain.Repositories.Write;
using ServerApp.Infrastructure.EF.Contexts;

/// <summary>
/// SQL Server implementation of IPaintingCategoryWriteRepository.
/// Handles only write operations using WriteDbContext.
/// </summary>
internal class SQLServerPaintingCategoryWriteRepository : IPaintingCategoryWriteRepository
{
    private readonly WriteDbContext _writeContext;

    public SQLServerPaintingCategoryWriteRepository(WriteDbContext writeContext)
    {
        _writeContext = writeContext;
    }

    public async Task AddAsync(PaintingCategory category, CancellationToken cancellationToken = default)
    {
        await _writeContext.PaintingCategories.AddAsync(category, cancellationToken);
    }

    public async Task UpdateAsync(PaintingCategory category, CancellationToken cancellationToken = default)
    {
        _writeContext.PaintingCategories.Update(category);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _writeContext.PaintingCategories.FindAsync(new object[] { id }, cancellationToken);
        if (category != null)
        {
            _writeContext.PaintingCategories.Remove(category);
        }
    }
}
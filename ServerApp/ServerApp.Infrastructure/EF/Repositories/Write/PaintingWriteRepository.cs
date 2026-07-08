namespace ServerApp.Infrastructure.EF.Repositories.Write;

using Microsoft.EntityFrameworkCore;
using ServerApp.Domain.Entities;
using ServerApp.Domain.Repositories.Write;
using ServerApp.Domain.ValueObjects.PaintingCategory;
using ServerApp.Infrastructure.EF.Contexts;

/// <summary>
/// EF Core implementation of IPaintingWriteRepository.
/// Handles only write operations using WriteDbContext.
/// </summary>
internal class PaintingWriteRepository : IPaintingWriteRepository
{
    private readonly WriteDbContext _writeContext;

    public PaintingWriteRepository(WriteDbContext writeContext)
    {
        _writeContext = writeContext;
    }

    public async Task<Painting?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _writeContext.Paintings.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Painting>> GetByCategoryAsync(PaintingCategorySlug categorySlug, CancellationToken cancellationToken = default)
    {
        return await _writeContext.Paintings
            .Where(p => p.CategorySlug == categorySlug.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Painting painting, CancellationToken cancellationToken = default)
    {
        await _writeContext.Paintings.AddAsync(painting, cancellationToken);
    }

    public async Task UpdateAsync(Painting painting, CancellationToken cancellationToken = default)
    {
        _writeContext.Paintings.Update(painting);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var painting = await _writeContext.Paintings.FindAsync(new object[] { id }, cancellationToken);
        if (painting != null)
        {
            _writeContext.Paintings.Remove(painting);
        }
    }
}

namespace ServerApp.Infrastructure.EF.Repositories.Read;

using Microsoft.EntityFrameworkCore;
using ServerApp.Domain.Entities;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Domain.ValueObjects.Painting;
using ServerApp.Domain.ValueObjects.PaintingCategory;
using ServerApp.Infrastructure.EF.Contexts;

/// <summary>
/// SQL Server implementation of IPaintingReadRepository.
/// Handles only read operations using ReadDbContext.
/// </summary>
internal class SQLServerPaintingReadRepository : IPaintingReadRepository
{
    private readonly ReadDbContext _readContext;

    public SQLServerPaintingReadRepository(ReadDbContext readContext)
    {
        _readContext = readContext;
    }

    public async Task<Painting?> GetAsync(PaintingSlug slug, CancellationToken cancellationToken = default)
    {
        return await _readContext.Paintings
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Slug == slug.Value, cancellationToken);
    }

    public async Task<IEnumerable<Painting>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _readContext.Paintings
            .Include(p => p.Category)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Painting>> GetByCategoryAsync(PaintingCategorySlug categorySlug, CancellationToken cancellationToken = default)
    {
        return await _readContext.Paintings
            .Include(p => p.Category)
            .Where(p => p.CategorySlug == categorySlug.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _readContext.Paintings.AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsBySlugAsync(PaintingSlug slug, CancellationToken cancellationToken = default)
    {
        return await _readContext.Paintings.AnyAsync(p => p.Slug == slug.Value, cancellationToken);
    }
}
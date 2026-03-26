namespace ServerApp.Infrastructure.EF.Repositories.Read;

using Microsoft.EntityFrameworkCore;
using ServerApp.Domain.Entities;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Domain.ValueObjects.PaintingCategory;
using ServerApp.Infrastructure.EF.Contexts;

/// <summary>
/// SQL Server implementation of IPaintingCategoryReadRepository.
/// Handles only read operations using ReadDbContext.
/// </summary>
internal class SQLServerPaintingCategoryReadRepository : IPaintingCategoryReadRepository
{
    private readonly ReadDbContext _readContext;

    public SQLServerPaintingCategoryReadRepository(ReadDbContext readContext)
    {
        _readContext = readContext;
    }

    public async Task<PaintingCategory?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _readContext.PaintingCategories
            .Include(c => c.Paintings)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<PaintingCategory?> FindBySlugAsync(PaintingCategorySlug slug, CancellationToken cancellationToken = default)
    {
        return await _readContext.PaintingCategories
            .Include(c => c.Paintings)
            .FirstOrDefaultAsync(c => c.Slug == slug.Value, cancellationToken);
    }

    public async Task<IEnumerable<PaintingCategory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _readContext.PaintingCategories
            .Include(c => c.Paintings)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(PaintingCategoryName name, CancellationToken cancellationToken = default)
    {
        return await _readContext.PaintingCategories.AnyAsync(c => c.Name == name.Value, cancellationToken);
    }
}
namespace ServerApp.Infrastructure.EF.Repositories;

using Microsoft.EntityFrameworkCore;
using ServerApp.Domain.Entities;
using ServerApp.Domain.Repositories;
using ServerApp.Domain.ValueObjects.PaintingCategory;
using ServerApp.Infrastructure.EF.Contexts;
using ServerApp.Infrastructure.EF.Models;
using ServerApp.Shared.Extensions;

internal class SQLServerPaintingCategoryRepository : IPaintingCategoryRepository
{
    private readonly ReadDbContext _readContext;
    private readonly WriteDbContext _writeContext;

    public SQLServerPaintingCategoryRepository(ReadDbContext readContext, WriteDbContext writeContext)
    {
        _readContext = readContext;
        _writeContext = writeContext;
    }

    public async Task<PaintingCategory?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var readModel = await _readContext.PaintingCategories
            .Include(c => c.Paintings)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (readModel == null)
            return null;

        return MapToDomain(readModel);
    }

    public async Task<PaintingCategory?> FindBySlugAsync(PaintingCategorySlug slug, CancellationToken cancellationToken = default)
    {
        var readModel = await _readContext.PaintingCategories
            .Include(c => c.Paintings)
            .FirstOrDefaultAsync(c => c.Slug == slug.Value, cancellationToken);

        if (readModel == null)
            return null;

        return MapToDomain(readModel);
    }

    public async Task<IEnumerable<PaintingCategory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var readModels = await _readContext.PaintingCategories
            .Include(c => c.Paintings)
            .ToListAsync(cancellationToken);

        return readModels.Select(MapToDomain);
    }

    public async Task AddAsync(PaintingCategory category, CancellationToken cancellationToken = default)
    {
        var writeModel = MapToWriteModel(category);
        await _writeContext.PaintingCategories.AddAsync(writeModel, cancellationToken);
        await _writeContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(PaintingCategory category, CancellationToken cancellationToken = default)
    {
        var writeModel = MapToWriteModel(category);
        _writeContext.PaintingCategories.Update(writeModel);
        await _writeContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var writeModel = await _writeContext.PaintingCategories.FindAsync(new object[] { id }, cancellationToken);
        if (writeModel != null)
        {
            _writeContext.PaintingCategories.Remove(writeModel);
            await _writeContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsByNameAsync(PaintingCategoryName name, CancellationToken cancellationToken = default)
    {
        return await _readContext.PaintingCategories.AnyAsync(c => c.Name == name.Value, cancellationToken);
    }

    private PaintingCategory MapToDomain(PaintingCategoryReadModel readModel)
    {
        var category = Activator.CreateInstance<PaintingCategory>();
        category.SetProtectedProperty("Id", readModel.Id);
        category.SetProtectedProperty("Name", new PaintingCategoryName(readModel.Name));
        category.SetProtectedProperty("Slug", new PaintingCategorySlug(readModel.Slug));
        category.SetProtectedProperty("Description", PaintingCategoryDescription.FromNullable(readModel.Description));

        return category;
    }

    private PaintingCategoryWriteModel MapToWriteModel(PaintingCategory category)
    {
        return new PaintingCategoryWriteModel
        {
            Id = category.Id,
            Name = category.Name.Value,
            Slug = category.Slug.Value,
            Description = category.Description?.Value
        };
    }
}
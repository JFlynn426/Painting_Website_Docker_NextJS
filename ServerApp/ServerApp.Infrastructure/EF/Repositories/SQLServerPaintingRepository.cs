namespace ServerApp.Infrastructure.EF.Repositories;

using Microsoft.EntityFrameworkCore;
using ServerApp.Domain.Entities;
using ServerApp.Domain.Repositories;
using ServerApp.Domain.ValueObjects.Painting;
using ServerApp.Domain.ValueObjects.PaintingCategory;
using ServerApp.Infrastructure.EF.Contexts;
using ServerApp.Infrastructure.EF.Models;

internal class SQLServerPaintingRepository : IPaintingRepository
{
    private readonly ReadDbContext _readContext;
    private readonly WriteDbContext _writeContext;

    public SQLServerPaintingRepository(ReadDbContext readContext, WriteDbContext writeContext)
    {
        _readContext = readContext;
        _writeContext = writeContext;
    }

    public async Task<Painting?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var readModel = await _readContext.Paintings
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (readModel == null)
            return null;

        return MapToDomain(readModel);
    }

    public async Task<IEnumerable<Painting>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var readModels = await _readContext.Paintings
            .Include(p => p.Category)
            .ToListAsync(cancellationToken);

        return readModels.Select(MapToDomain);
    }

    public async Task<IEnumerable<Painting>> GetByCategoryAsync(PaintingCategorySlug categorySlug, CancellationToken cancellationToken = default)
    {
        var readModels = await _readContext.Paintings
            .Include(p => p.Category)
            .Where(p => p.CategorySlug == categorySlug.Value)
            .ToListAsync(cancellationToken);

        return readModels.Select(MapToDomain);
    }

    public async Task AddAsync(Painting painting, CancellationToken cancellationToken = default)
    {
        var writeModel = MapToWriteModel(painting);
        await _writeContext.Paintings.AddAsync(writeModel, cancellationToken);
        await _writeContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Painting painting, CancellationToken cancellationToken = default)
    {
        var writeModel = MapToWriteModel(painting);
        _writeContext.Paintings.Update(writeModel);
        await _writeContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var writeModel = await _writeContext.Paintings.FindAsync(new object[] { id }, cancellationToken);
        if (writeModel != null)
        {
            _writeContext.Paintings.Remove(writeModel);
            await _writeContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _readContext.Paintings.AnyAsync(p => p.Id == id, cancellationToken);
    }

    private Painting MapToDomain(PaintingReadModel readModel)
    {
        var painting = Activator.CreateInstance<Painting>();
        SetProtectedProperty(painting, "Id", readModel.Id);
        SetProtectedProperty(painting, "Title", new PaintingName(readModel.Title));
        SetProtectedProperty(painting, "Description", new PaintingDescription(readModel.Description ?? string.Empty));
        SetProtectedProperty(painting, "ImageUrl", new PaintingImageUrl(readModel.ImageUrl));
        SetProtectedProperty(painting, "ThumbnailUrl", new PaintingThumbnailUrl(readModel.ThumbnailUrl ?? string.Empty));
        SetProtectedProperty(painting, "CategorySlug", new PaintingCategorySlug(readModel.CategorySlug));
        SetProtectedProperty(painting, "Width", readModel.Width.HasValue ? new PaintingWidth(readModel.Width.Value) : null);
        SetProtectedProperty(painting, "Height", readModel.Height.HasValue ? new PaintingHeight(readModel.Height.Value) : null);
        SetProtectedProperty(painting, "Depth", readModel.Depth.HasValue ? new PaintingDepth(readModel.Depth.Value) : null);
        SetProtectedProperty(painting, "Year", readModel.Year.HasValue ? new PaintingYear(readModel.Year.Value) : null);
        SetProtectedProperty(painting, "Price", readModel.Price.HasValue ? new PaintingPrice(readModel.Price.Value) : null);
        SetProtectedProperty(painting, "IsAvailable", new PaintingIsAvailable(readModel.IsAvailable));

        return painting;
    }

    private PaintingWriteModel MapToWriteModel(Painting painting)
    {
        return new PaintingWriteModel
        {
            Id = painting.Id,
            Title = painting.Title.Value,
            Description = painting.Description.Value,
            ImageUrl = painting.ImageUrl.Value,
            ThumbnailUrl = painting.ThumbnailUrl.Value,
            CategorySlug = painting.CategorySlug.Value,
            Width = painting.Width?.Value,
            Height = painting.Height?.Value,
            Depth = painting.Depth?.Value,
            Year = painting.Year?.Value,
            Price = painting.Price?.Value,
            IsAvailable = painting.IsAvailable.Value
        };
    }

    private void SetProtectedProperty(object obj, string propertyName, object? value)
    {
        var property = obj.GetType().GetProperty(propertyName);
        property?.SetValue(obj, value);
    }
}
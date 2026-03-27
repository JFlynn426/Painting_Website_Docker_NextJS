namespace ServerApp.Domain.Factories;

using ServerApp.Domain.Entities;
using ServerApp.Domain.ValueObjects.PaintingCategory;

public interface IPaintingCategoryFactory
{
    Task<PaintingCategory> CreateAsync(
        PaintingCategoryName name,
        PaintingCategoryDescription? description,
        CancellationToken cancellationToken = default);
}
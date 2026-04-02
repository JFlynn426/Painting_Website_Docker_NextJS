namespace ServerApp.Domain.Factories;

using ServerApp.Domain.Entities;

public interface IPaintingCategoryFactory
{
    Task<PaintingCategory> CreateAsync(
        string name,
        string? description,
        CancellationToken cancellationToken = default);
}
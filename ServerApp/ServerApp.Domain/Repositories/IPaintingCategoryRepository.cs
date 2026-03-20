namespace ServerApp.Domain.Repositories;

using ServerApp.Domain.Entities;
using ServerApp.Domain.ValueObjects.PaintingCategory;

public interface IPaintingCategoryRepository
{
    Task<PaintingCategory?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(PaintingCategory category, CancellationToken cancellationToken = default);
    Task UpdateAsync(PaintingCategory category, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(PaintingCategoryName name, CancellationToken cancellationToken = default);
}

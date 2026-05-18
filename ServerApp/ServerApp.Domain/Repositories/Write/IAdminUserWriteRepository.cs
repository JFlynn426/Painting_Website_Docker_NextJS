namespace ServerApp.Domain.Repositories.Write;

using ServerApp.Domain.Entities;

/// <summary>
/// Write repository interface for AdminUser entities.
/// Handles only write operations (Add, Update).
/// </summary>
public interface IAdminUserWriteRepository
{
    Task AddAsync(AdminUser adminUser, CancellationToken cancellationToken = default);
    Task UpdateAsync(AdminUser adminUser, CancellationToken cancellationToken = default);
}

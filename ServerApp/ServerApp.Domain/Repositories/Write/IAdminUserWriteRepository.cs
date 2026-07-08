namespace ServerApp.Domain.Repositories.Write;

using ServerApp.Domain.Entities;

/// <summary>
/// Write repository interface for AdminUser entities.
/// Handles only write operations (Add, Update).
/// </summary>
public interface IAdminUserWriteRepository
{
    Task<AdminUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AdminUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(AdminUser adminUser, CancellationToken cancellationToken = default);
    Task UpdateAsync(AdminUser adminUser, CancellationToken cancellationToken = default);
}

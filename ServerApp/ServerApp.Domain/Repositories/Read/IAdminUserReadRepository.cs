namespace ServerApp.Domain.Repositories.Read;

using ServerApp.Domain.Entities;

/// <summary>
/// Read repository interface for AdminUser entities.
/// Handles only read operations (Get, Exists).
/// </summary>
public interface IAdminUserReadRepository
{
    Task<AdminUser?> GetByGoogleSubjectIdAsync(string googleSubjectId, CancellationToken cancellationToken = default);
    Task<AdminUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<AdminUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

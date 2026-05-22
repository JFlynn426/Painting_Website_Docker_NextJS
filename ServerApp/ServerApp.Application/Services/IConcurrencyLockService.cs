namespace ServerApp.Application.Services;

/// <summary>
/// Service for managing per-user concurrency locks to prevent simultaneous mutations.
/// </summary>
public interface IConcurrencyLockService
{
    /// <summary>
    /// Attempts to acquire a lock for the given admin user.
    /// </summary>
    /// <param name="adminId">The admin user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if lock was acquired, false if another operation is in progress.</returns>
    Task<bool> TryAcquireAsync(Guid adminId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Releases the lock for the given admin user.
    /// </summary>
    /// <param name="adminId">The admin user ID.</param>
    Task ReleaseAsync(Guid adminId);
}

namespace ServerApp.Application.Services;

/// <summary>
/// Service for managing idempotency keys to allow safe command retries.
/// </summary>
public interface IIdempotencyKeyService
{
    /// <summary>
    /// Checks if the given idempotency key is new (not yet processed).
    /// </summary>
    Task<bool> IsKeyNewAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores the idempotency key with its result for future retries.
    /// </summary>
    Task StoreKeyWithResultAsync(string key, object result, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a previously stored result for the given idempotency key.
    /// </summary>
    Task<object?> GetStoredResultAsync(string key, CancellationToken cancellationToken = default);
}

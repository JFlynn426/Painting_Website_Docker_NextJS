namespace ServerApp.Infrastructure.Services;

using System.Collections.Concurrent;
using ServerApp.Application.Services;

/// <summary>
/// In-memory implementation of concurrency locking using SemaphoreSlim per user.
/// </summary>
public class ConcurrencyLockService : IConcurrencyLockService
{
    private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _locks = new();
    private readonly TimeSpan _timeout = TimeSpan.FromSeconds(30);

    public async Task<bool> TryAcquireAsync(Guid adminId, CancellationToken cancellationToken = default)
    {
        var semaphore = _locks.GetOrAdd(adminId, _ => new SemaphoreSlim(1, 1));
        return await semaphore.WaitAsync(_timeout, cancellationToken);
    }

    public Task ReleaseAsync(Guid adminId)
    {
        if (_locks.TryGetValue(adminId, out var semaphore))
        {
            semaphore.Release();
        }
        return Task.CompletedTask;
    }
}

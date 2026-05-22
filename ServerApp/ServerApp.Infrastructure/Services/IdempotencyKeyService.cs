namespace ServerApp.Infrastructure.Services;

using System.Collections.Concurrent;
using ServerApp.Application.DTOs;
using ServerApp.Application.Services;

/// <summary>
/// In-memory implementation of idempotency key storage with sliding expiration.
/// </summary>
public class IdempotencyKeyService : IIdempotencyKeyService
{
    private readonly ConcurrentDictionary<string, IdempotencyEntry> _store = new();
    private readonly TimeSpan _expiration = TimeSpan.FromMinutes(15);

    public Task<bool> IsKeyNewAsync(string key, CancellationToken cancellationToken = default)
    {
        if (_store.TryGetValue(key, out var entry))
        {
            if (DateTime.UtcNow - entry.CreatedAt < _expiration)
            {
                return Task.FromResult(false);
            }
            else
            {
                _store.TryRemove(key, out _);
            }
        }
        return Task.FromResult(true);
    }

    public Task StoreKeyWithResultAsync(string key, object result, CancellationToken cancellationToken = default)
    {
        _store[key] = new IdempotencyEntry(result);
        return Task.CompletedTask;
    }

    public Task<object?> GetStoredResultAsync(string key, CancellationToken cancellationToken = default)
    {
        if (_store.TryGetValue(key, out var entry))
        {
            if (DateTime.UtcNow - entry.CreatedAt < _expiration)
            {
                return Task.FromResult((object?)entry.Result);
            }
            else
            {
                _store.TryRemove(key, out _);
            }
        }
        return Task.FromResult<object?>(null);
    }

    private class IdempotencyEntry
    {
        public object Result { get; }
        public DateTime CreatedAt { get; }

        public IdempotencyEntry(object result)
        {
            Result = result;
            CreatedAt = DateTime.UtcNow;
        }
    }
}

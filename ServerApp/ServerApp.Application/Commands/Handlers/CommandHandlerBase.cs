namespace ServerApp.Application.Commands.Handlers;

using ServerApp.Application.DTOs;
using ServerApp.Application.Exceptions;
using ServerApp.Application.Services;

/// <summary>
/// Abstract base class for command handlers that provides concurrency locking, idempotency, timeout handling, and completion responses.
/// </summary>
public abstract class CommandHandlerBase
{
    protected readonly IConcurrencyLockService _concurrencyLock;
    protected readonly IIdempotencyKeyService _idempotencyKey;

    private static readonly TimeSpan CommandTimeout = TimeSpan.FromSeconds(90);

    protected CommandHandlerBase(IConcurrencyLockService concurrencyLock, IIdempotencyKeyService idempotencyKey)
    {
        _concurrencyLock = concurrencyLock;
        _idempotencyKey = idempotencyKey;
    }

    protected async Task<CommandCompletionResponse> ExecuteAsync(
        Guid adminId,
        string? idempotencyKey,
        Func<CancellationToken, Task<int>> action,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(adminId, idempotencyKey,
            async ct => { var count = await action(ct); return (count, (string?)null); },
            cancellationToken);
    }

    protected async Task<CommandCompletionResponse> ExecuteAsync(
        Guid adminId,
        string? idempotencyKey,
        Func<CancellationToken, Task<(int affectedRecords, string? newSlug)>> action,
        CancellationToken cancellationToken = default)
    {
        // 1. Check idempotency
        if (!string.IsNullOrEmpty(idempotencyKey))
        {
            var cached = await _idempotencyKey.GetStoredResultAsync(idempotencyKey, cancellationToken);
            if (cached is CommandCompletionResponse response)
            {
                return response;
            }
        }

        // 2. Acquire concurrency lock
        var lockAcquired = await _concurrencyLock.TryAcquireAsync(adminId, cancellationToken);
        if (!lockAcquired)
        {
            throw new ConcurrentUpdateException();
        }

        // 3. Create timeout token source linked with the original cancellation token
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(CommandTimeout);

        try
        {
            // 4. Execute command with timeout
            var (affectedRecords, newSlug) = await action(timeoutCts.Token);

            // 5. Build response
            var result = new CommandCompletionResponse
            {
                Success = true,
                Message = "Command completed successfully",
                CompletedAt = DateTime.UtcNow,
                AffectedRecords = affectedRecords,
                NewSlug = newSlug
            };

            // 6. Cache for idempotency
            if (!string.IsNullOrEmpty(idempotencyKey))
            {
                await _idempotencyKey.StoreKeyWithResultAsync(idempotencyKey, result, cancellationToken);
            }

            return result;
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
            throw new CommandTimeoutException();
        }
        finally
        {
            await _concurrencyLock.ReleaseAsync(adminId);
        }
    }
}

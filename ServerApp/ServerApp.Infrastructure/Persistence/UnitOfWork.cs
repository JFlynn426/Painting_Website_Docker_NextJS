namespace ServerApp.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore.Storage;
using ServerApp.Infrastructure.EF.Contexts;
using ServerApp.Shared.Persistence;

/// <summary>
/// Unit of Work implementation using Entity Framework Core's WriteDbContext.
/// Manages database transactions and commits.
/// </summary>
internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly WriteDbContext _writeDbContext;
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    public UnitOfWork(WriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    /// <inheritdoc/>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            return; // Transaction already started
        }

        _transaction = await _writeDbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            // No transaction started, just save changes
            await _writeDbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        await _writeDbContext.SaveChangesAsync(cancellationToken);
        await _transaction.CommitAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    /// <inheritdoc/>
    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            return; // No transaction to rollback
        }

        await _transaction.RollbackAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        // Dispose transaction if it exists and hasn't been committed/rolled back
        // This handles the edge case where Dispose is called without Commit/Rollback
        if (_transaction != null)
        {
            try
            {
                _transaction.Dispose();
            }
            catch
            {
                // Ignore disposal errors
            }
            _transaction = null;
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
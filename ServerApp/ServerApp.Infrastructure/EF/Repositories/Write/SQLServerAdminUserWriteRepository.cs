namespace ServerApp.Infrastructure.EF.Repositories.Write;

using Microsoft.EntityFrameworkCore;
using ServerApp.Domain.Entities;
using ServerApp.Domain.Repositories.Write;
using ServerApp.Infrastructure.EF.Contexts;

/// <summary>
/// SQL Server implementation of IAdminUserWriteRepository.
/// Handles only write operations using WriteDbContext.
/// </summary>
internal class SQLServerAdminUserWriteRepository : IAdminUserWriteRepository
{
    private readonly WriteDbContext _writeContext;

    public SQLServerAdminUserWriteRepository(WriteDbContext writeContext)
    {
        _writeContext = writeContext;
    }

    public async Task AddAsync(AdminUser adminUser, CancellationToken cancellationToken = default)
    {
        await _writeContext.AdminUsers.AddAsync(adminUser, cancellationToken);
    }

    public async Task UpdateAsync(AdminUser adminUser, CancellationToken cancellationToken = default)
    {
        _writeContext.AdminUsers.Update(adminUser);
    }
}

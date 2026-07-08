namespace ServerApp.Infrastructure.EF.Repositories.Write;

using Microsoft.EntityFrameworkCore;
using ServerApp.Domain.Entities;
using ServerApp.Domain.Repositories.Write;
using ServerApp.Domain.ValueObjects.Admin;
using ServerApp.Infrastructure.EF.Contexts;

/// <summary>
/// EF Core implementation of IAdminUserWriteRepository.
/// Handles only write operations using WriteDbContext.
/// </summary>
internal class AdminUserWriteRepository : IAdminUserWriteRepository
{
    private readonly WriteDbContext _writeContext;

    public AdminUserWriteRepository(WriteDbContext writeContext)
    {
        _writeContext = writeContext;
    }

    public async Task<AdminUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _writeContext.AdminUsers.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<AdminUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _writeContext.AdminUsers
            .FirstOrDefaultAsync(u => u.Email == new AdminEmail(email), cancellationToken);
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

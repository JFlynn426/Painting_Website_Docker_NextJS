namespace ServerApp.Infrastructure.EF.Repositories.Read;

using Microsoft.EntityFrameworkCore;
using ServerApp.Domain.Entities;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Infrastructure.EF.Contexts;

/// <summary>
/// EF Core implementation of IAdminUserReadRepository.
/// Handles only read operations using ReadDbContext.
/// </summary>
internal class AdminUserReadRepository : IAdminUserReadRepository
{
    private readonly ReadDbContext _readContext;

    public AdminUserReadRepository(ReadDbContext readContext)
    {
        _readContext = readContext;
    }

    public async Task<AdminUser?> GetByGoogleSubjectIdAsync(string googleSubjectId, CancellationToken cancellationToken = default)
    {
        return await _readContext.AdminUsers
            .FirstOrDefaultAsync(u => u.GoogleSubjectId == googleSubjectId, cancellationToken);
    }

    public async Task<AdminUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _readContext.AdminUsers
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<AdminUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _readContext.AdminUsers
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }
}

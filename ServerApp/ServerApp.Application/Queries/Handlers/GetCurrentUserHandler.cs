namespace ServerApp.Application.Queries.Handlers;

using MediatR;
using ServerApp.Application.Queries;
using ServerApp.Application.DTOs;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Application.Exceptions;

public class GetCurrentUserHandler : IRequestHandler<GetCurrentUser, AdminUserDto?>
{
    private readonly IAdminUserReadRepository _readRepository;

    public GetCurrentUserHandler(IAdminUserReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<AdminUserDto?> Handle(GetCurrentUser query, CancellationToken cancellationToken = default)
    {
        var adminUser = await _readRepository.GetByIdAsync(query.AdminId, cancellationToken);

        if (adminUser == null)
        {
            return null;
        }

        return new AdminUserDto
        {
            Id = adminUser.Id,
            Email = adminUser.Email.Value,
            DisplayName = adminUser.DisplayName.Value,
            PictureUrl = adminUser.PictureUrl?.Value,
            LastLoginAt = adminUser.LastLoginAt.Value,
            CreatedAt = adminUser.CreatedAt.Value,
            IsActive = adminUser.IsActive.Value
        };
    }
}

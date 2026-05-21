namespace ServerApp.Application.Commands;

using MediatR;

public record UpdateAdminUser(
    Guid Id,
    string? DisplayName,
    string? PictureUrl,
    bool? IsActive
) : IRequest;

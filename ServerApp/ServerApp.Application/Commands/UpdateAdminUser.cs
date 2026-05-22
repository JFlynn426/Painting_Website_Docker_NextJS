namespace ServerApp.Application.Commands;

using MediatR;
using ServerApp.Application.DTOs;

public record UpdateAdminUser(
    Guid Id,
    string? DisplayName,
    string? PictureUrl,
    bool? IsActive,
    Guid AdminId,
    string? IdempotencyKey
) : IRequest<CommandCompletionResponse>;

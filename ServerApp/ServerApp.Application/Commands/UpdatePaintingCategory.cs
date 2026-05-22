namespace ServerApp.Application.Commands;

using MediatR;
using ServerApp.Application.DTOs;

public record UpdatePaintingCategory(
    Guid Id,
    string? Name,
    string? Description,
    Guid AdminId,
    string? IdempotencyKey
) : IRequest<CommandCompletionResponse>;

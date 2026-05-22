namespace ServerApp.Application.Commands;

using MediatR;
using ServerApp.Application.DTOs;

public record AssignPaintingCategory(
    Guid PaintingId,
    Guid CategoryId,
    Guid AdminId,
    string? IdempotencyKey
) : IRequest<CommandCompletionResponse>;

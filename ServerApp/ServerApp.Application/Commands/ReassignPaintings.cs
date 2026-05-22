namespace ServerApp.Application.Commands;

using MediatR;
using ServerApp.Application.DTOs;

public record ReassignPaintings(
    Dictionary<Guid, Guid> PaintingIdToCategoryId,
    Guid AdminId,
    string? IdempotencyKey
) : IRequest<CommandCompletionResponse>;

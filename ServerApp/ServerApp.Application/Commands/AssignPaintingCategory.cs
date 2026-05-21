namespace ServerApp.Application.Commands;

using MediatR;

public record AssignPaintingCategory(
    Guid PaintingId,
    Guid CategoryId
) : IRequest;

namespace ServerApp.Application.Commands;

using MediatR;

public record ReassignPaintings(
    Dictionary<Guid, Guid> PaintingIdToCategoryId
) : IRequest;

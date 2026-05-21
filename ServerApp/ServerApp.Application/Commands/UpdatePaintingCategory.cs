namespace ServerApp.Application.Commands;

using MediatR;

public record UpdatePaintingCategory(
    Guid Id,
    string? Name,
    string? Description
) : IRequest;

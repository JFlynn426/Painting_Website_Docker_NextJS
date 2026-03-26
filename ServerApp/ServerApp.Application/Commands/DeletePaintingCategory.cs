namespace ServerApp.Application.Commands;

using MediatR;

public record DeletePaintingCategory(Guid Id) : IRequest;
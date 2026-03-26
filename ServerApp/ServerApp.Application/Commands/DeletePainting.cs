namespace ServerApp.Application.Commands;

using MediatR;

public record DeletePainting(Guid Id) : IRequest;
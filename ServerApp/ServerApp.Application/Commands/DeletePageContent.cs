namespace ServerApp.Application.Commands;

using MediatR;

public record DeletePageContent(string Address) : IRequest;
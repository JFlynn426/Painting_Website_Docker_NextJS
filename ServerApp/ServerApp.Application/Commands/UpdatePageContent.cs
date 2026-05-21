namespace ServerApp.Application.Commands;

using MediatR;

public record UpdatePageContent(
    Guid Id,
    string? Title,
    string? Content
) : IRequest;

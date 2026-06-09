namespace ServerApp.Application.Commands;

using MediatR;
using ServerApp.Application.DTOs;

public record UpdatePageContent(
    Guid Id,
    string? Title,
    string? Content,
    string[]? PhotoUrls,
    Guid AdminId,
    string? IdempotencyKey
) : IRequest<CommandCompletionResponse>;

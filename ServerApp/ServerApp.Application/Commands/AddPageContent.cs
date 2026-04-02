namespace ServerApp.Application.Commands;

using MediatR;
using ServerApp.Application.DTOs;

public record AddPageContent(
    string Address,
    string? Title,
    string Content
) : IRequest<PageContentCreatedResult>;
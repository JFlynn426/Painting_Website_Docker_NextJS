namespace ServerApp.Application.Commands;

using MediatR;
using ServerApp.Application.DTOs;

public record UpdatePainting(
    Guid Id,
    string? Name,
    string? Description,
    string? ImageUrl,
    string? ThumbnailUrl,
    string? Slug,
    string? CategoryId,
    int? Width,
    int? Height,
    int? Depth,
    int? Year,
    decimal? Price,
    bool? IsAvailable,
    bool? IsNew,
    bool? IsCarouselPainting,
    Guid AdminId,
    string? IdempotencyKey
) : IRequest<CommandCompletionResponse>;

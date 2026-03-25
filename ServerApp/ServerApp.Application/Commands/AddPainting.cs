namespace ServerApp.Application.Commands;

using ServerApp.Shared.Abstractions.Commands;

public record AddPainting(
    string Title,
    string? Description,
    string ImageUrl,
    string? ThumbnailUrl,
    string CategorySlug,
    decimal? Price,
    decimal? Width,
    decimal? Height,
    decimal? Depth,
    int? Year,
    bool IsAvailable
) : ICommand;
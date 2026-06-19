namespace ServerApp.Domain.Services;

/// <summary>
/// Result of image processing operation containing URLs for all generated image sizes.
/// </summary>
public record ImageProcessingResult(
    string OriginalUrl,
    string HighResUrl,
    string ThumbnailUrl
);

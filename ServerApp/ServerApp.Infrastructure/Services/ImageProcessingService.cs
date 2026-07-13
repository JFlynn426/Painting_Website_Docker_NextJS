namespace ServerApp.Infrastructure.Services;

using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using ServerApp.Domain.Services;

/// <summary>
/// Image processing service implementation using SixLabors.ImageSharp.
/// Processes uploaded images into multiple sizes (original, high-res, thumbnail).
/// </summary>
public class ImageProcessingService : IImageProcessingService
{
    private readonly int _highResMaxEdge;
    private readonly int _thumbnailMaxEdge;
    private readonly int _highResQuality;
    private readonly int _thumbnailQuality;
    private readonly long _maxFileSizeBytes;
    private readonly string _storagePath;

    public ImageProcessingService(IConfiguration configuration)
    {
        _highResMaxEdge = int.Parse(configuration["ImageProcessing:HighResMaxEdge"] ?? "2500");
        _thumbnailMaxEdge = int.Parse(configuration["ImageProcessing:ThumbnailMaxEdge"] ?? "800");
        _highResQuality = int.Parse(configuration["ImageProcessing:HighResQuality"] ?? "92");
        _thumbnailQuality = int.Parse(configuration["ImageProcessing:ThumbnailQuality"] ?? "85");
        _maxFileSizeBytes = long.Parse(configuration["ImageProcessing:MaxFileSizeMb"] ?? "20") * 1024 * 1024;
        _storagePath = configuration["ImageProcessing:StoragePath"] ?? "/app/images";
    }

    public async Task<ImageProcessingResult> ProcessAndSaveAsync(Stream imageStream, string fileName, CancellationToken cancellationToken = default)
    {
        // Validate file size
        if (imageStream.Length > _maxFileSizeBytes)
        {
            throw new InvalidOperationException($"File size exceeds maximum allowed size of {_maxFileSizeBytes / 1024 / 1024}MB");
        }

        // Generate safe filename
        var safeFileName = Path.GetFileNameWithoutExtension(fileName);
        var extension = Path.GetExtension(fileName)?.ToLowerInvariant() ?? ".jpg";

        // Validate file extension
        if (extension != ".jpg" && extension != ".jpeg")
        {
            throw new InvalidOperationException("Only JPG/JPEG files are allowed");
        }

        // Generate unique filename using GUID
        var uniqueId = Guid.NewGuid().ToString("N");
        var sanitizedFileName = SanitizeFileName(safeFileName);
        var baseFileName = $"{uniqueId}_{sanitizedFileName}";

        // Ensure directories exist
        var originalDir = Path.Combine(_storagePath, "original");
        var highResDir = Path.Combine(_storagePath, "high-res");
        var thumbnailDir = Path.Combine(_storagePath, "thumbnail");

        Directory.CreateDirectory(originalDir);
        Directory.CreateDirectory(highResDir);
        Directory.CreateDirectory(thumbnailDir);

        // Load image from stream
        using var image = await Image.LoadAsync(imageStream, cancellationToken);

        // Validate dimensions (prevent billion laughs attack)
        if (image.Width > 20000 || image.Height > 20000)
        {
            throw new InvalidOperationException("Image dimensions exceed maximum allowed size of 20000x20000 pixels");
        }

        // Detect orientation from JPEG pixel dimensions
        bool isLandscape = image.Width > image.Height;

        // Save original
        var originalPath = Path.Combine(originalDir, $"{baseFileName}{extension}");
        await image.SaveAsJpegAsync(originalPath, new JpegEncoder { Quality = 95 }, cancellationToken);

        // Resize to high-res (2500px long edge)
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max,
            Size = new Size(_highResMaxEdge, _highResMaxEdge)
        }));
        var highResPath = Path.Combine(highResDir, $"{baseFileName}{extension}");
        await image.SaveAsJpegAsync(highResPath, new JpegEncoder { Quality = _highResQuality }, cancellationToken);

        // Resize to thumbnail (800px long edge)
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max,
            Size = new Size(_thumbnailMaxEdge, _thumbnailMaxEdge)
        }));
        var thumbnailPath = Path.Combine(thumbnailDir, $"{baseFileName}{extension}");
        await image.SaveAsJpegAsync(thumbnailPath, new JpegEncoder { Quality = _thumbnailQuality }, cancellationToken);

        return new ImageProcessingResult(
            OriginalUrl: $"/images/original/{Path.GetFileName(originalPath)}",
            HighResUrl: $"/images/high-res/{Path.GetFileName(highResPath)}",
            ThumbnailUrl: $"/images/thumbnail/{Path.GetFileName(thumbnailPath)}",
            IsLandscape: isLandscape
        );
    }

    public async Task DeleteAsync(string fileName, CancellationToken cancellationToken = default)
    {
        // Search in all directories and delete matching files
        var directories = new[] { "original", "high-res", "thumbnail" };
        foreach (var dir in directories)
        {
            var dirPath = Path.Combine(_storagePath, dir);
            var fullPath = Path.Combine(dirPath, fileName);

            if (File.Exists(fullPath))
            {
                await Task.Run(() => File.Delete(fullPath), cancellationToken);
            }
        }
    }

    public bool? GetImageIsLandscapeFromUrl(string imageUrl)
    {
        // URL format: /images/{size}/{filename} where size is "original", "high-res", or "thumbnail"
        // Extract the filename from the URL
        var uri = imageUrl.TrimStart('/');
        var parts = uri.Split('/');
        if (parts.Length < 3)
        {
            return null;
        }

        // parts[0] = "images", parts[1] = size, parts[2+] = filename
        var size = parts[1];
        var fileName = string.Join("/", parts.Skip(2));
        var fullPath = Path.Combine(_storagePath, size, fileName);

        if (!File.Exists(fullPath))
        {
            // If file doesn't exist, return null so isLandscape is not changed
            return null;
        }

        using var image = Image.Load(fullPath);
        return image.Width > image.Height;
    }

    /// <summary>
    /// Sanitizes a filename to be URL-safe by replacing spaces with hyphens
    /// and removing characters that are unsafe in URLs.
    /// </summary>
    private static string SanitizeFileName(string fileName)
    {
        // Replace spaces with hyphens
        var sanitized = fileName.Replace(' ', '-');

        // Remove characters that are unsafe in URLs (keep alphanumeric, hyphens, underscores, dots)
        sanitized = Regex.Replace(sanitized, @"[^a-zA-Z0-9\-\._]", "");

        // Collapse multiple consecutive hyphens/underscores/dots into one
        sanitized = Regex.Replace(sanitized, @"[-]{2,}", "-");
        sanitized = Regex.Replace(sanitized, @"[_]{2,}", "_");
        sanitized = Regex.Replace(sanitized, @"[.]{2,}", ".");

        // Remove leading/trailing hyphens, underscores, dots
        sanitized = sanitized.TrimStart('-', '_', '.').TrimEnd('-', '_', '.');

        // Truncate if too long (keep under 100 chars for filename safety)
        if (sanitized.Length > 100)
        {
            sanitized = sanitized[..100];
            sanitized = sanitized.TrimEnd('-', '_', '.');
        }

        return sanitized;
    }
}

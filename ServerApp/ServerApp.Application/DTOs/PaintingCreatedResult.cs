namespace ServerApp.Application.DTOs;

/// <summary>
/// Result returned after successfully creating a painting.
/// </summary>
public record PaintingCreatedResult(
    Guid Id,
    string Slug,
    string Title
);
namespace ServerApp.Application.DTOs;

/// <summary>
/// Result returned after successfully creating a painting category.
/// </summary>
public record PaintingCategoryCreatedResult(
    Guid Id,
    string Slug,
    string Name
);
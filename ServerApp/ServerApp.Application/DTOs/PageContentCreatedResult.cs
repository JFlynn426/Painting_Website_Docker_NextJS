namespace ServerApp.Application.DTOs;

/// <summary>
/// Result returned after successfully creating page content.
/// </summary>
public record PageContentCreatedResult(
    Guid Id,
    string Address,
    string Title
);
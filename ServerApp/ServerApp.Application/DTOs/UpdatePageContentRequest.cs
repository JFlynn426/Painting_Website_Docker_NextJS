namespace ServerApp.Application.DTOs;

/// <summary>
/// Request DTO for updating page content.
/// </summary>
public class UpdatePageContentRequest
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string[]? PhotoUrls { get; set; }
}

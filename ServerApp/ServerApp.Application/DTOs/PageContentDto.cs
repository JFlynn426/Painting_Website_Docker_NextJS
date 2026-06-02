namespace ServerApp.Application.DTOs;

public class PageContentDto
{
    public Guid Id { get; init; }
    public string Address { get; init; } = string.Empty;
    public string? Title { get; init; }
    public string Content { get; init; } = string.Empty;
    public string? PhotoUrl { get; init; }
}

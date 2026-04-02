namespace ServerApp.Application.DTOs;

public class PageContentDto
{
    public string Address { get; init; } = string.Empty;
    public string? Title { get; init; }
    public string Content { get; init; } = string.Empty;
}
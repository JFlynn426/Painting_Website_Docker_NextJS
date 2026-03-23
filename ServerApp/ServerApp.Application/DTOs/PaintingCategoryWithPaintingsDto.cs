namespace ServerApp.Application.DTOs;

public class PaintingCategoryWithPaintingsDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string? Description { get; init; }
    public List<PaintingDto> Paintings { get; init; } = new();
}
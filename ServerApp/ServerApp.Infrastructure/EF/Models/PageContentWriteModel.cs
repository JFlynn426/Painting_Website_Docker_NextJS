namespace ServerApp.Infrastructure.EF.Models;

public class PageContentWriteModel
{
    public Guid Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
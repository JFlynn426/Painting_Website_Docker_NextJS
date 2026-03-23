namespace ServerApp.Domain.Entities;

using ServerApp.Shared.Abstractions.Domain;
using ServerApp.Domain.ValueObjects.Page;

public class PageContent : AggregateRoot<Guid>
{
    public PageAddress Address { get; protected set; }
    public PageTitle Title { get; protected set; }
    public PageContentText Content { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }

    // Constructor for creating a new page content
    internal PageContent(PageAddress address, PageTitle title, PageContentText content)
    {
        Address = address;
        Title = title;
        Content = content;
    }

    // Parameterless constructor for ORM
    protected PageContent() { }

    // Method to update page content
    public void UpdateContent(PageTitle newTitle, PageContentText newContent)
    {
        Title = newTitle;
        Content = newContent;
        UpdatedAt = DateTime.UtcNow;
    }
}
namespace ServerApp.Domain.Entities;

using ServerApp.Shared.Domain;
using ServerApp.Domain.ValueObjects.Page;

public class PageContent : AggregateRoot<Guid>
{
    public PageAddress Address { get; private set; }
    public PageTitle Title { get; private set; }
    public PageContentText Content { get; private set; }

    // Parameterless constructor for EF Core
    private PageContent() { }

    // Constructor for creating a new page content (domain creation path)
    internal PageContent(PageAddress address, PageTitle title, PageContentText content)
    {
        Address = address;
        Title = title;
        Content = content;
    }

    // Method to update page content
    public void UpdateContent(PageTitle newTitle, PageContentText newContent)
    {
        Title = newTitle;
        Content = newContent;
    }
}
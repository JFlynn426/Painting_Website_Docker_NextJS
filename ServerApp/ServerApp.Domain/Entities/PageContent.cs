namespace ServerApp.Domain.Entities;

using ServerApp.Shared.Domain;
using ServerApp.Domain.ValueObjects.Page;
using ServerApp.Domain.Events;

public class PageContent : AggregateRoot<Guid>
{
    public PageAddress Address { get; private set; }
    public PageTitle? Title { get; private set; }
    public PageContentText Content { get; private set; }

    // Parameterless constructor for EF Core
    private PageContent() { }

    // Constructor for creating a new page content (domain creation path)
    internal PageContent(PageAddress address, PageTitle? title, PageContentText content)
    {
        Address = address;
        Title = title;
        Content = content;

        AddEvent(new PageContentCreatedEvent(Id, address.Value));
    }

    // Consolidated update method - applies only non-null parameters
    public void Update(
        PageTitle? title = null,
        PageContentText? content = null)
    {
        if (title != null) Title = title;
        if (content != null) Content = content;

        AddEvent(new PageContentUpdatedEvent(Id, Address.Value));
    }
}
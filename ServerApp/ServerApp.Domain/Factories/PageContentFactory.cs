namespace ServerApp.Domain.Factories;

using ServerApp.Domain.Entities;
using ServerApp.Domain.ValueObjects.Page;

public class PageContentFactory : IPageContentFactory
{
    public PageContent Create(
        PageAddress address,
        PageTitle? title,
        PageContentText content,
        PagePhotoUrl? photoUrl = null)
    {
        return new PageContent(address, title, content, photoUrl);
    }
}

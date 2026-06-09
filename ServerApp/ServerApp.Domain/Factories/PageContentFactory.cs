namespace ServerApp.Domain.Factories;

using ServerApp.Domain.Entities;
using ServerApp.Domain.ValueObjects.Page;

public class PageContentFactory : IPageContentFactory
{
    public PageContent Create(
        PageAddress address,
        PageTitle? title,
        PageContentText content,
        PagePhotoUrls? photoUrls = null)
    {
        return new PageContent(address, title, content, photoUrls);
    }
}

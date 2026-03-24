namespace ServerApp.Domain.Factories;

using ServerApp.Domain.Entities;
using ServerApp.Domain.Repositories;
using ServerApp.Domain.ValueObjects.Page;

public class PageContentFactory : IPageContentFactory
{
    private readonly IPageContentRepository _repository;

    public PageContentFactory(IPageContentRepository repository)
    {
        _repository = repository;
    }

    public PageContent Create(
        PageAddress address,
        PageTitle title,
        PageContentText content)
    {
        return new PageContent(address, title, content);
    }
}
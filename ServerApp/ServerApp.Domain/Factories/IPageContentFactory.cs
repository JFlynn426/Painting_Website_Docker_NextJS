namespace ServerApp.Domain.Factories;

using ServerApp.Domain.Entities;
using ServerApp.Domain.ValueObjects.Page;

public interface IPageContentFactory
{
    PageContent Create(
        PageAddress address,
        PageTitle title,
        PageContentText content);
}
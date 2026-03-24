namespace ServerApp.Infrastructure.EF.Repositories;

using Microsoft.EntityFrameworkCore;
using ServerApp.Domain.Entities;
using ServerApp.Domain.Repositories;
using ServerApp.Domain.ValueObjects.Page;
using ServerApp.Infrastructure.EF.Contexts;
using ServerApp.Infrastructure.EF.Models;
using ServerApp.Shared.Extensions;

internal class SQLServerPageContentRepository : IPageContentRepository
{
    private readonly ReadDbContext _readContext;
    private readonly WriteDbContext _writeContext;

    public SQLServerPageContentRepository(ReadDbContext readContext, WriteDbContext writeContext)
    {
        _readContext = readContext;
        _writeContext = writeContext;
    }

    public async Task<PageContent?> GetByAddressAsync(PageAddress address, CancellationToken cancellationToken = default)
    {
        var readModel = await _readContext.PageContents
            .FirstOrDefaultAsync(p => p.Address == address.Value, cancellationToken);

        if (readModel == null)
            return null;

        return MapToDomain(readModel);
    }

    public async Task AddAsync(PageContent pageContent, CancellationToken cancellationToken = default)
    {
        var writeModel = MapToWriteModel(pageContent);
        await _writeContext.PageContents.AddAsync(writeModel, cancellationToken);
        await _writeContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(PageContent pageContent, CancellationToken cancellationToken = default)
    {
        var writeModel = MapToWriteModel(pageContent);
        _writeContext.PageContents.Update(writeModel);
        await _writeContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByAddressAsync(PageAddress address, CancellationToken cancellationToken = default)
    {
        return await _readContext.PageContents.AnyAsync(p => p.Address == address.Value, cancellationToken);
    }

    public async Task DeleteAsync(PageAddress address, CancellationToken cancellationToken = default)
    {
        var writeModel = await _writeContext.PageContents
            .FirstOrDefaultAsync(p => p.Address == address.Value, cancellationToken);

        if (writeModel == null)
            return;

        _writeContext.PageContents.Remove(writeModel);
        await _writeContext.SaveChangesAsync(cancellationToken);
    }

    private PageContent MapToDomain(PageContentReadModel readModel)
    {
        var pageContent = Activator.CreateInstance<PageContent>();
        pageContent.SetProtectedProperty("Id", readModel.Id);
        pageContent.SetProtectedProperty("Address", new PageAddress(readModel.Address));
        pageContent.SetProtectedProperty("Title", new PageTitle(readModel.Title));
        pageContent.SetProtectedProperty("Content", new PageContentText(readModel.Content));
        pageContent.SetProtectedProperty("UpdatedAt", readModel.UpdatedAt);

        return pageContent;
    }

    private PageContentWriteModel MapToWriteModel(PageContent pageContent)
    {
        return new PageContentWriteModel
        {
            Id = pageContent.Id,
            Address = pageContent.Address.Value,
            Title = pageContent.Title.Value,
            Content = pageContent.Content.Value,
            UpdatedAt = pageContent.UpdatedAt
        };
    }
}
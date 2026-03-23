namespace ServerApp.Infrastructure.EF.Repositories;

using Microsoft.EntityFrameworkCore;
using ServerApp.Domain.Entities;
using ServerApp.Domain.Repositories;
using ServerApp.Domain.ValueObjects.Page;
using ServerApp.Infrastructure.EF.Contexts;
using ServerApp.Infrastructure.EF.Models;

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

    private PageContent MapToDomain(PageContentReadModel readModel)
    {
        var pageContent = Activator.CreateInstance<PageContent>();
        SetProtectedProperty(pageContent, "Id", readModel.Id);
        SetProtectedProperty(pageContent, "Address", new PageAddress(readModel.Address));
        SetProtectedProperty(pageContent, "Title", new PageTitle(readModel.Title));
        SetProtectedProperty(pageContent, "Content", new PageContentText(readModel.Content));
        SetProtectedProperty(pageContent, "UpdatedAt", readModel.UpdatedAt);

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

    private void SetProtectedProperty(object obj, string propertyName, object? value)
    {
        var property = obj.GetType().GetProperty(propertyName);
        property?.SetValue(obj, value);
    }
}
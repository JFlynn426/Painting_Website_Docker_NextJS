namespace ServerApp.Infrastructure.Queries.Handlers;

using Microsoft.EntityFrameworkCore;
using ServerApp.Shared.Abstractions.Queries;
using ServerApp.Application.Queries;
using ServerApp.Application.DTOs;
using ServerApp.Application.Exceptions;
using ServerApp.Infrastructure.EF.Contexts;
using ServerApp.Infrastructure.EF.Models;

internal sealed class GetPageContentHandler : IQueryHandler<GetPageContent, PageContentDto>
{
    private readonly DbSet<PageContentReadModel> _pageContents;

    public GetPageContentHandler(ReadDbContext readDbContext)
    {
        _pageContents = readDbContext.PageContents;
    }

    public async Task<PageContentDto> HandleAsync(GetPageContent query, CancellationToken cancellationToken = default)
    {
        var pageContent = await _pageContents
            .FirstOrDefaultAsync(pc => pc.Address == query.Address, cancellationToken);

        if (pageContent == null)
        {
            throw new PageContentNotFoundException(query.Address);
        }

        return pageContent.AsDTO();
    }
}
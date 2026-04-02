namespace ServerApp.Application.Queries.Handlers;

using MediatR;
using ServerApp.Application.Queries;
using ServerApp.Application.DTOs;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Application.Exceptions;
using ServerApp.Domain.ValueObjects.Page;

public class GetPageContentHandler : IRequestHandler<GetPageContent, PageContentDto>
{
    private readonly IPageContentReadRepository _readRepository;

    public GetPageContentHandler(IPageContentReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<PageContentDto> Handle(GetPageContent query, CancellationToken cancellationToken = default)
    {
        var address = new PageAddress(query.Address);
        var pageContent = await _readRepository.GetByAddressAsync(address, cancellationToken);

        if (pageContent == null)
        {
            throw new PageContentNotFoundException(query.Address);
        }

        return new PageContentDto
        {
            Address = pageContent.Address.Value,
            Title = pageContent.Title?.Value,
            Content = pageContent.Content.Value
        };
    }
}
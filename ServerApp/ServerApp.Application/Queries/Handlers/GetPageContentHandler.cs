namespace ServerApp.Application.Queries.Handlers;

using ServerApp.Shared.Abstractions.Queries;
using ServerApp.Application.Queries;
using ServerApp.Application.DTOs;
using ServerApp.Domain.Repositories;
using ServerApp.Domain.ValueObjects.Page;
using ServerApp.Application.Exceptions;

public class GetPageContentHandler : IQueryHandler<GetPageContent, PageContentDto>
{
    private readonly IPageContentRepository _repository;

    public GetPageContentHandler(IPageContentRepository repository)
    {
        _repository = repository;
    }

    public async Task<PageContentDto> HandleAsync(GetPageContent query)
    {
        var pageContent = await _repository.GetByAddressAsync(new PageAddress(query.Address), default);

        if (pageContent == null)
        {
            throw new PageContentNotFoundException(query.Address);
        }

        return new PageContentDto
        {
            Id = pageContent.Id,
            Address = pageContent.Address.Value,
            Title = pageContent.Title.Value,
            Content = pageContent.Content.Value,
            UpdatedAt = pageContent.UpdatedAt
        };
    }
}
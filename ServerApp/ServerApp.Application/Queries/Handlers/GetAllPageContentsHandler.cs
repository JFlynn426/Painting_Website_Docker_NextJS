namespace ServerApp.Application.Queries.Handlers;

using MediatR;
using ServerApp.Application.Queries;
using ServerApp.Application.DTOs;
using ServerApp.Domain.Repositories.Read;

public class GetAllPageContentsHandler : IRequestHandler<GetAllPageContents, List<PageContentDto>>
{
    private readonly IPageContentReadRepository _readRepository;

    public GetAllPageContentsHandler(IPageContentReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<List<PageContentDto>> Handle(GetAllPageContents query, CancellationToken cancellationToken = default)
    {
        var pageContents = await _readRepository.GetAllAsync(cancellationToken);

        return pageContents.Select(pc => new PageContentDto
        {
            Id = pc.Id,
            Address = pc.Address.Value,
            Title = pc.Title?.Value,
            Content = pc.Content.Value,
            PhotoUrl = pc.PhotoUrl?.Value
        }).ToList();
    }
}
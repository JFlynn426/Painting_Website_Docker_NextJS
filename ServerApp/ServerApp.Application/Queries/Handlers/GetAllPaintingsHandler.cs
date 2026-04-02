namespace ServerApp.Application.Queries.Handlers;

using MediatR;
using ServerApp.Application.Queries;
using ServerApp.Application.DTOs;
using ServerApp.Domain.Repositories.Read;

public class GetAllPaintingsHandler : IRequestHandler<GetAllPaintings, List<PaintingDto>>
{
    private readonly IPaintingReadRepository _readRepository;

    public GetAllPaintingsHandler(IPaintingReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<List<PaintingDto>> Handle(GetAllPaintings query, CancellationToken cancellationToken = default)
    {
        var paintings = await _readRepository.GetAllAsync(cancellationToken);

        return paintings.Select(p => new PaintingDto
        {
            Id = p.Id,
            Slug = p.Slug.Value,
            Title = p.Title.Value,
            Description = p.Description?.Value,
            ImageUrl = p.ImageUrl.Value,
            ThumbnailUrl = p.ThumbnailUrl?.Value,
            CategorySlug = p.CategorySlug.Value,
            Width = p.Width?.Value,
            Height = p.Height?.Value,
            Depth = p.Depth?.Value,
            Year = p.Year?.Value,
            Price = p.Price?.Value,
            IsAvailable = p.IsAvailable.Value,
            IsNew = p.IsNew.Value
        }).ToList();
    }
}
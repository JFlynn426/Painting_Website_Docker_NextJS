namespace ServerApp.Application.Queries.Handlers;

using MediatR;
using ServerApp.Application.Queries;
using ServerApp.Application.DTOs;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Application.Exceptions;
using ServerApp.Domain.ValueObjects.Painting;

public class GetPaintingHandler : IRequestHandler<GetPainting, PaintingDto>
{
    private readonly IPaintingReadRepository _readRepository;

    public GetPaintingHandler(IPaintingReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<PaintingDto> Handle(GetPainting query, CancellationToken cancellationToken = default)
    {
        var slug = new PaintingSlug(query.Slug);
        var painting = await _readRepository.GetAsync(slug, cancellationToken);

        if (painting == null)
        {
            throw new PaintingNotFoundException(query.Slug);
        }

        return new PaintingDto
        {
            Id = painting.Id,
            Title = painting.Title.Value,
            Description = painting.Description?.Value,
            ImageUrl = painting.ImageUrl.Value,
            ThumbnailUrl = painting.ThumbnailUrl?.Value,
            CategorySlug = painting.CategorySlug.Value,
            Width = painting.Width?.Value,
            Height = painting.Height?.Value,
            Depth = painting.Depth?.Value,
            Year = painting.Year?.Value,
            Price = painting.Price?.Value,
            IsAvailable = painting.IsAvailable.Value
        };
    }
}
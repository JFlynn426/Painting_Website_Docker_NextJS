namespace ServerApp.Application.Queries.Handlers;

using ServerApp.Shared.Abstractions.Queries;
using ServerApp.Application.Queries;
using ServerApp.Application.DTOs;
using ServerApp.Domain.Repositories;
using ServerApp.Application.Exceptions;
using ServerApp.Domain.ValueObjects.Painting;

public class GetPaintingHandler : IQueryHandler<GetPainting, PaintingDto>
{
    private readonly IPaintingRepository _repository;

    public GetPaintingHandler(IPaintingRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaintingDto> HandleAsync(GetPainting query, CancellationToken cancellationToken = default)
    {
        var slug = new PaintingSlug(query.Slug);
        var painting = await _repository.GetAsync(slug, cancellationToken);

        if (painting == null)
        {
            throw new PaintingNotFoundException(query.Slug);
        }

        return new PaintingDto
        {
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
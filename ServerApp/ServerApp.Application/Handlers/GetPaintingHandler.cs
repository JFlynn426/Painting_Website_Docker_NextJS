namespace ServerApp.Application.Handlers;

using ServerApp.Shared.Abstractions.Queries;
using ServerApp.Application.Queries;
using ServerApp.Application.DTOs;
using ServerApp.Domain.Repositories;
using ServerApp.Application.Exceptions;

public class GetPaintingHandler : IQueryHandler<GetPainting, PaintingDto>
{
    private readonly IPaintingRepository _repository;

    public GetPaintingHandler(IPaintingRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaintingDto> HandleAsync(GetPainting query)
    {
        var painting = await _repository.GetAsync(query.Id, default);

        if (painting == null)
        {
            throw new PaintingNotFoundException(query.Id);
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
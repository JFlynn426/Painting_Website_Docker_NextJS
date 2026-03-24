namespace ServerApp.Application.Queries.Handlers;

using ServerApp.Shared.Abstractions.Queries;
using ServerApp.Application.Queries;
using ServerApp.Application.DTOs;
using ServerApp.Domain.Repositories;
using ServerApp.Domain.ValueObjects.PaintingCategory;
using ServerApp.Application.Exceptions;

public class GetPaintingCategoryWithPaintingsHandler : IQueryHandler<GetPaintingCategoryWithPaintings, PaintingCategoryWithPaintingsDto>
{
    private readonly IPaintingCategoryRepository _categoryRepository;
    private readonly IPaintingRepository _paintingRepository;

    public GetPaintingCategoryWithPaintingsHandler(
        IPaintingCategoryRepository categoryRepository,
        IPaintingRepository paintingRepository)
    {
        _categoryRepository = categoryRepository;
        _paintingRepository = paintingRepository;
    }

    public async Task<PaintingCategoryWithPaintingsDto> HandleAsync(GetPaintingCategoryWithPaintings query, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.FindBySlugAsync(new PaintingCategorySlug(query.Slug), cancellationToken);

        if (category == null)
        {
            throw new PaintingCategoryNotFoundException(query.Slug);
        }

        var paintings = await _paintingRepository.GetByCategoryAsync(category.Slug, cancellationToken);

        return new PaintingCategoryWithPaintingsDto
        {
            Id = category.Id,
            Name = category.Name.Value,
            Slug = category.Slug.Value,
            Description = category.Description?.Value,
            Paintings = paintings.Select(p => new PaintingDto
            {
                Id = p.Id,
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
                IsAvailable = p.IsAvailable.Value
            }).ToList()
        };
    }
}
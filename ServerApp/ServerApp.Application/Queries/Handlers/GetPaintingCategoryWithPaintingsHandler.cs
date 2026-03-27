namespace ServerApp.Application.Queries.Handlers;

using MediatR;
using ServerApp.Application.Queries;
using ServerApp.Application.DTOs;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Application.Exceptions;
using ServerApp.Domain.ValueObjects.PaintingCategory;

public class GetPaintingCategoryWithPaintingsHandler : IRequestHandler<GetPaintingCategoryWithPaintings, PaintingCategoryWithPaintingsDto>
{
    private readonly IPaintingCategoryReadRepository _categoryReadRepository;
    private readonly IPaintingReadRepository _paintingReadRepository;

    public GetPaintingCategoryWithPaintingsHandler(
        IPaintingCategoryReadRepository categoryReadRepository,
        IPaintingReadRepository paintingReadRepository)
    {
        _categoryReadRepository = categoryReadRepository;
        _paintingReadRepository = paintingReadRepository;
    }

    public async Task<PaintingCategoryWithPaintingsDto> Handle(GetPaintingCategoryWithPaintings query, CancellationToken cancellationToken = default)
    {
        var slug = new PaintingCategorySlug(query.Slug);
        var category = await _categoryReadRepository.FindBySlugAsync(slug, cancellationToken);

        if (category == null)
        {
            throw new PaintingCategoryNotFoundException(query.Slug);
        }

        var paintings = await _paintingReadRepository.GetByCategoryAsync(slug, cancellationToken);

        return new PaintingCategoryWithPaintingsDto
        {
            Id = category.Id,
            Name = category.Name.Value,
            Slug = category.Slug.Value,
            Description = category.Description?.Value,
            Paintings = paintings.Select(p => new PaintingDto
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
                IsAvailable = p.IsAvailable.Value
            }).ToList()
        };
    }
}
namespace ServerApp.Application.Queries.Handlers;

using ServerApp.Shared.Abstractions.Queries;
using ServerApp.Application.Queries;
using ServerApp.Application.DTOs;
using ServerApp.Domain.Repositories;

public class GetAllPaintingCategoriesHandler : IQueryHandler<GetAllPaintingCategories, List<PaintingCategoryDto>>
{
    private readonly IPaintingCategoryRepository _repository;

    public GetAllPaintingCategoriesHandler(IPaintingCategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<PaintingCategoryDto>> HandleAsync(GetAllPaintingCategories query, CancellationToken cancellationToken = default)
    {
        var categories = await _repository.GetAllAsync(cancellationToken);
        return categories.Select(c => new PaintingCategoryDto
        {
            Id = c.Id,
            Name = c.Name.Value,
            Slug = c.Slug.Value,
            Description = c.Description?.Value
        }).ToList();
    }
}
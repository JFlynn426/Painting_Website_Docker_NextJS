namespace ServerApp.Application.Queries.Handlers;

using MediatR;
using ServerApp.Application.Queries;
using ServerApp.Application.DTOs;
using ServerApp.Domain.Repositories.Read;

public class GetAllPaintingCategoriesHandler : IRequestHandler<GetAllPaintingCategories, List<PaintingCategoryDto>>
{
    private readonly IPaintingCategoryReadRepository _readRepository;

    public GetAllPaintingCategoriesHandler(IPaintingCategoryReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<List<PaintingCategoryDto>> Handle(GetAllPaintingCategories query, CancellationToken cancellationToken = default)
    {
        var categories = await _readRepository.GetAllAsync(cancellationToken);

        return categories.Select(c => new PaintingCategoryDto
        {
            Id = c.Id,
            Name = c.Name.Value,
            Slug = c.Slug.Value,
            Description = c.Description
        }).ToList();
    }
}
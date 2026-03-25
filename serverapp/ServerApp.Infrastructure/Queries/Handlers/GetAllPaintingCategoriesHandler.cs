namespace ServerApp.Infrastructure.Queries.Handlers;

using Microsoft.EntityFrameworkCore;
using ServerApp.Shared.Abstractions.Queries;
using ServerApp.Application.Queries;
using ServerApp.Application.DTOs;
using ServerApp.Infrastructure.EF.Contexts;
using ServerApp.Infrastructure.EF.Models;

internal sealed class GetAllPaintingCategoriesHandler : IQueryHandler<GetAllPaintingCategories, List<PaintingCategoryDto>>
{
    private readonly DbSet<PaintingCategoryReadModel> _paintingCategories;

    public GetAllPaintingCategoriesHandler(ReadDbContext readDbContext)
    {
        _paintingCategories = readDbContext.PaintingCategories;
    }

    public async Task<List<PaintingCategoryDto>> HandleAsync(GetAllPaintingCategories query, CancellationToken cancellationToken = default)
    {
        var categories = await _paintingCategories.ToListAsync(cancellationToken);

        return categories.Select(c => c.AsDTO()).ToList();
    }
}
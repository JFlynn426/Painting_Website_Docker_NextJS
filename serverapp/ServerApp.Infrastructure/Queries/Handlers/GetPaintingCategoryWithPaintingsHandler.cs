namespace ServerApp.Infrastructure.Queries.Handlers;

using Microsoft.EntityFrameworkCore;
using ServerApp.Shared.Abstractions.Queries;
using ServerApp.Application.Queries;
using ServerApp.Application.DTOs;
using ServerApp.Application.Exceptions;
using ServerApp.Infrastructure.EF.Contexts;
using ServerApp.Infrastructure.EF.Models;

internal sealed class GetPaintingCategoryWithPaintingsHandler : IQueryHandler<GetPaintingCategoryWithPaintings, PaintingCategoryWithPaintingsDto>
{
    private readonly DbSet<PaintingCategoryReadModel> _paintingCategories;
    private readonly DbSet<PaintingReadModel> _paintings;

    public GetPaintingCategoryWithPaintingsHandler(ReadDbContext readDbContext)
    {
        _paintingCategories = readDbContext.PaintingCategories;
        _paintings = readDbContext.Paintings;
    }

    public async Task<PaintingCategoryWithPaintingsDto> HandleAsync(GetPaintingCategoryWithPaintings query, CancellationToken cancellationToken = default)
    {
        var category = await _paintingCategories
            .FirstOrDefaultAsync(pc => pc.Slug == query.Slug, cancellationToken);

        if (category == null)
        {
            throw new PaintingCategoryNotFoundException(query.Slug);
        }

        var paintings = await _paintings
            .Where(p => p.CategorySlug == query.Slug)
            .ToListAsync(cancellationToken);

        return category.AsDTOWithPaintings(paintings);
    }
}
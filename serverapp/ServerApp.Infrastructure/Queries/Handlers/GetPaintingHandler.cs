namespace ServerApp.Infrastructure.Queries.Handlers;

using Microsoft.EntityFrameworkCore;
using ServerApp.Shared.Abstractions.Queries;
using ServerApp.Application.Queries;
using ServerApp.Application.DTOs;
using ServerApp.Application.Exceptions;
using ServerApp.Infrastructure.EF.Contexts;
using ServerApp.Infrastructure.EF.Models;

internal sealed class GetPaintingHandler : IQueryHandler<GetPainting, PaintingDto>
{
    private readonly DbSet<PaintingReadModel> _paintings;

    public GetPaintingHandler(ReadDbContext readDbContext)
    {
        _paintings = readDbContext.Paintings;
    }

    public async Task<PaintingDto> HandleAsync(GetPainting query, CancellationToken cancellationToken = default)
    {
        var painting = await _paintings
            .FirstOrDefaultAsync(p => p.Slug == query.Slug, cancellationToken);

        if (painting == null)
        {
            throw new PaintingNotFoundException(query.Slug);
        }

        return painting.AsDTO();
    }
}
namespace ServerApp.Infrastructure.Commands.Handlers;

using Microsoft.EntityFrameworkCore;
using ServerApp.Shared.Abstractions.Commands;
using ServerApp.Application.Commands;
using ServerApp.Application.Exceptions;
using ServerApp.Infrastructure.EF.Contexts;
using ServerApp.Infrastructure.EF.Models;
using ServerApp.Infrastructure.Commands;
using ServerApp.Domain.ValueObjects.Painting;

internal sealed class AddPaintingHandler : ICommandHandler<AddPainting>
{
    private readonly WriteDbContext _writeDbContext;

    public AddPaintingHandler(WriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    public async Task HandleAsync(AddPainting command, CancellationToken cancellationToken = default)
    {
        var title = new PaintingName(command.Title);
        var slug = PaintingSlug.FromTitle(title);

        var existingPainting = await _writeDbContext.Paintings
            .FirstOrDefaultAsync(p => p.Slug == slug.Value, cancellationToken);

        if (existingPainting != null)
        {
            throw new PaintingSlugAlreadyExistsException();
        }

        var category = await _writeDbContext.PaintingCategories
            .FirstOrDefaultAsync(pc => pc.Slug == command.CategorySlug, cancellationToken);

        if (category == null)
        {
            throw new PaintingCategoryNotFoundException(command.CategorySlug);
        }

        var painting = command.ToWriteModel();
        painting.CategoryId = category.Id;

        await _writeDbContext.Paintings.AddAsync(painting, cancellationToken);
        await _writeDbContext.SaveChangesAsync(cancellationToken);
    }
}
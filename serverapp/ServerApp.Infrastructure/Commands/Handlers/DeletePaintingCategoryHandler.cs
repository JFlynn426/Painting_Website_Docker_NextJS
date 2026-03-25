namespace ServerApp.Infrastructure.Commands.Handlers;

using Microsoft.EntityFrameworkCore;
using ServerApp.Shared.Abstractions.Commands;
using ServerApp.Application.Commands;
using ServerApp.Infrastructure.EF.Contexts;
using ServerApp.Infrastructure.EF.Models;
using ServerApp.Domain.ValueObjects.PaintingCategory;

internal sealed class DeletePaintingCategoryHandler : ICommandHandler<DeletePaintingCategory>
{
    private readonly WriteDbContext _writeDbContext;

    public DeletePaintingCategoryHandler(WriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    public async Task HandleAsync(DeletePaintingCategory command, CancellationToken cancellationToken = default)
    {
        var id = new PaintingCategoryID(command.Id);

        var category = await _writeDbContext.PaintingCategories
            .FirstOrDefaultAsync(pc => pc.Id == id.Value, cancellationToken);

        if (category != null)
        {
            _writeDbContext.PaintingCategories.Remove(category);
            await _writeDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
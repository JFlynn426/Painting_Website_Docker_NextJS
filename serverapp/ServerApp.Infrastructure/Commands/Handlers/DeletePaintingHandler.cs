namespace ServerApp.Infrastructure.Commands.Handlers;

using Microsoft.EntityFrameworkCore;
using ServerApp.Shared.Abstractions.Commands;
using ServerApp.Application.Commands;
using ServerApp.Infrastructure.EF.Contexts;
using ServerApp.Infrastructure.EF.Models;
using ServerApp.Domain.ValueObjects.Painting;

internal sealed class DeletePaintingHandler : ICommandHandler<DeletePainting>
{
    private readonly WriteDbContext _writeDbContext;

    public DeletePaintingHandler(WriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    public async Task HandleAsync(DeletePainting command, CancellationToken cancellationToken = default)
    {
        var id = new PaintingID(command.Id);

        var painting = await _writeDbContext.Paintings
            .FirstOrDefaultAsync(p => p.Id == id.Value, cancellationToken);

        if (painting != null)
        {
            _writeDbContext.Paintings.Remove(painting);
            await _writeDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
namespace ServerApp.Infrastructure.Commands.Handlers;

using Microsoft.EntityFrameworkCore;
using ServerApp.Shared.Abstractions.Commands;
using ServerApp.Application.Commands;
using ServerApp.Infrastructure.EF.Contexts;
using ServerApp.Infrastructure.EF.Models;
using ServerApp.Domain.ValueObjects.Page;

internal sealed class DeletePageContentHandler : ICommandHandler<DeletePageContent>
{
    private readonly WriteDbContext _writeDbContext;

    public DeletePageContentHandler(WriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    public async Task HandleAsync(DeletePageContent command, CancellationToken cancellationToken = default)
    {
        var address = new PageAddress(command.Address);

        var pageContent = await _writeDbContext.PageContents
            .FirstOrDefaultAsync(pc => pc.Address == address.Value, cancellationToken);

        if (pageContent != null)
        {
            _writeDbContext.PageContents.Remove(pageContent);
            await _writeDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
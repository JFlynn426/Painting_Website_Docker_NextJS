namespace ServerApp.Infrastructure.Commands.Handlers;

using Microsoft.EntityFrameworkCore;
using ServerApp.Shared.Abstractions.Commands;
using ServerApp.Application.Commands;
using ServerApp.Infrastructure.EF.Contexts;
using ServerApp.Infrastructure.EF.Models;
using ServerApp.Infrastructure.Commands;

internal sealed class AddPageContentHandler : ICommandHandler<AddPageContent>
{
    private readonly WriteDbContext _writeDbContext;

    public AddPageContentHandler(WriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    public async Task HandleAsync(AddPageContent command, CancellationToken cancellationToken = default)
    {
        var existingPageContent = await _writeDbContext.PageContents
            .FirstOrDefaultAsync(pc => pc.Address == command.Address, cancellationToken);

        if (existingPageContent != null)
        {
            // Update existing record
            existingPageContent.Title = command.Title;
            existingPageContent.Content = command.Content;
            _writeDbContext.PageContents.Update(existingPageContent);
        }
        else
        {
            // Insert new record
            var pageContent = command.ToWriteModel();
            await _writeDbContext.PageContents.AddAsync(pageContent, cancellationToken);
        }

        await _writeDbContext.SaveChangesAsync(cancellationToken);
    }
}
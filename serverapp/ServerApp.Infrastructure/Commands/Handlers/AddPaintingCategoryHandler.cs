namespace ServerApp.Infrastructure.Commands.Handlers;

using Microsoft.EntityFrameworkCore;
using ServerApp.Shared.Abstractions.Commands;
using ServerApp.Application.Commands;
using ServerApp.Infrastructure.EF.Contexts;
using ServerApp.Infrastructure.EF.Models;
using ServerApp.Infrastructure.Commands;

internal sealed class AddPaintingCategoryHandler : ICommandHandler<AddPaintingCategory>
{
    private readonly WriteDbContext _writeDbContext;

    public AddPaintingCategoryHandler(WriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    public async Task HandleAsync(AddPaintingCategory command, CancellationToken cancellationToken = default)
    {
        var category = command.ToWriteModel();

        await _writeDbContext.PaintingCategories.AddAsync(category, cancellationToken);
        await _writeDbContext.SaveChangesAsync(cancellationToken);
    }
}
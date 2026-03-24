namespace ServerApp.Application.Commands.Handlers;

using ServerApp.Shared.Abstractions.Commands;
using ServerApp.Application.Commands;
using ServerApp.Domain.Repositories;

public class DeletePaintingCategoryHandler : ICommandHandler<DeletePaintingCategory>
{
    private readonly IPaintingCategoryRepository _repository;

    public DeletePaintingCategoryHandler(IPaintingCategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(DeletePaintingCategory command, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(command.Id, cancellationToken);
    }
}
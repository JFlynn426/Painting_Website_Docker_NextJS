namespace ServerApp.Application.Commands.Handlers;

using ServerApp.Shared.Abstractions.Commands;
using ServerApp.Application.Commands;
using ServerApp.Domain.Repositories;

public class DeletePaintingHandler : ICommandHandler<DeletePainting>
{
    private readonly IPaintingRepository _repository;

    public DeletePaintingHandler(IPaintingRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(DeletePainting command, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(command.Id, cancellationToken);
    }
}
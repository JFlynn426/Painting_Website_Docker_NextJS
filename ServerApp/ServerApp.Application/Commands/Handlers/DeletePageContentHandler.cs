namespace ServerApp.Application.Commands.Handlers;

using ServerApp.Application.Commands;
using ServerApp.Domain.Repositories;
using ServerApp.Domain.ValueObjects.Page;
using ServerApp.Shared.Abstractions.Commands;

public class DeletePageContentHandler : ICommandHandler<DeletePageContent>
{
    private readonly IPageContentRepository _repository;

    public DeletePageContentHandler(IPageContentRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(DeletePageContent command, CancellationToken cancellationToken = default)
    {
        var address = new PageAddress(command.Address);
        await _repository.DeleteAsync(address, cancellationToken);
    }
}
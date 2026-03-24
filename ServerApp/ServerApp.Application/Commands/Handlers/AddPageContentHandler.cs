namespace ServerApp.Application.Commands.Handlers;

using ServerApp.Shared.Abstractions.Commands;
using ServerApp.Application.Commands;
using ServerApp.Domain.Factories;
using ServerApp.Domain.Repositories;
using ServerApp.Domain.ValueObjects.Page;

public class AddPageContentHandler : ICommandHandler<AddPageContent>
{
    private readonly IPageContentFactory _factory;
    private readonly IPageContentRepository _repository;

    public AddPageContentHandler(
        IPageContentFactory factory,
        IPageContentRepository repository)
    {
        _factory = factory;
        _repository = repository;
    }

    public async Task HandleAsync(AddPageContent command, CancellationToken cancellationToken = default)
    {
        var (address, title, content) = command;

        var pageContent = _factory.Create(address, title, content);
        await _repository.AddAsync(pageContent, cancellationToken);
    }
}
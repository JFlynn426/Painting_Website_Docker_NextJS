namespace ServerApp.Application.Commands.Handlers;

using ServerApp.Shared.Abstractions.Commands;
using ServerApp.Application.Commands;
using ServerApp.Domain.Factories;
using ServerApp.Domain.Repositories;
using ServerApp.Domain.ValueObjects.PaintingCategory;

public class AddPaintingCategoryHandler : ICommandHandler<AddPaintingCategory>
{
    private readonly IPaintingCategoryFactory _factory;
    private readonly IPaintingCategoryRepository _repository;

    public AddPaintingCategoryHandler(
        IPaintingCategoryFactory factory,
        IPaintingCategoryRepository repository)
    {
        _factory = factory;
        _repository = repository;
    }

    public async Task HandleAsync(AddPaintingCategory command, CancellationToken cancellationToken = default)
    {
        var (id, name, description) = command;

        var category = await _factory.CreateAsync(
            id,
            name,
            PaintingCategoryDescription.FromNullable(description),
            cancellationToken);

        await _repository.AddAsync(category, cancellationToken);
    }
}
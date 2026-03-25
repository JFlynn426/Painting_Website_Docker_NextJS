namespace ServerApp.Application.Commands.Handlers;

using ServerApp.Shared.Abstractions.Commands;
using ServerApp.Application.Commands;
using ServerApp.Domain.Factories;
using ServerApp.Domain.Repositories;
using ServerApp.Domain.ValueObjects.Painting;
using ServerApp.Domain.ValueObjects.PaintingCategory;
using ServerApp.Application.Exceptions;

public class AddPaintingHandler : ICommandHandler<AddPainting>
{
    private readonly IPaintingFactory _factory;
    private readonly IPaintingRepository _repository;

    public AddPaintingHandler(
        IPaintingFactory factory,
        IPaintingRepository repository)
    {
        _factory = factory;
        _repository = repository;
    }

    public async Task HandleAsync(AddPainting command, CancellationToken cancellationToken = default)
    {
        var (title, description, imageUrl, thumbnailUrl, categorySlug, price, width, height, depth, year, isAvailable) = command;

        // Generate slug from title
        var paintingName = new PaintingName(title);
        var slug = PaintingSlug.FromTitle(paintingName);

        // Check if a painting with this slug already exists
        var exists = await _repository.ExistsBySlugAsync(slug, cancellationToken);
        if (exists)
        {
            throw new PaintingSlugAlreadyExistsException();
        }

        // Auto-generate ID
        var id = new PaintingID();

        var painting = await _factory.CreateAsync(
            id,
            title,
            PaintingDescription.FromNullable(description),
            imageUrl,
            PaintingThumbnailUrl.FromNullable(thumbnailUrl),
            categorySlug,
            price,
            width,
            height,
            depth,
            year,
            isAvailable,
            cancellationToken);

        await _repository.AddAsync(painting, cancellationToken);
    }
}
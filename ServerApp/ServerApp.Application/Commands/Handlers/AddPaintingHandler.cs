namespace ServerApp.Application.Commands.Handlers;

using MediatR;
using ServerApp.Shared.Persistence;
using ServerApp.Application.Commands;
using ServerApp.Application.DTOs;
using ServerApp.Domain.Factories;
using ServerApp.Domain.Repositories.Write;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Domain.ValueObjects.Painting;
using ServerApp.Domain.ValueObjects.PaintingCategory;
using ServerApp.Application.Exceptions;
using ServerApp.Domain.Exceptions;

public class AddPaintingHandler : IRequestHandler<AddPainting, PaintingCreatedResult>
{
    private readonly IPaintingFactory _factory;
    private readonly IPaintingWriteRepository _paintingWriteRepository;
    private readonly IPaintingReadRepository _paintingReadRepository;
    private readonly IPaintingCategoryReadRepository _categoryReadRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddPaintingHandler(
        IPaintingFactory factory,
        IPaintingWriteRepository paintingWriteRepository,
        IPaintingReadRepository paintingReadRepository,
        IPaintingCategoryReadRepository categoryReadRepository,
        IUnitOfWork unitOfWork)
    {
        _factory = factory;
        _paintingWriteRepository = paintingWriteRepository;
        _paintingReadRepository = paintingReadRepository;
        _categoryReadRepository = categoryReadRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PaintingCreatedResult> Handle(AddPainting command, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var (title, description, imageUrl, thumbnailUrl, categorySlug, price, width, height, depth, year, isAvailable) = command;

            // Validate that the category exists (moved from factory to handler)
            var categorySlugVO = new PaintingCategorySlug(categorySlug);
            var category = await _categoryReadRepository.FindBySlugAsync(categorySlugVO, cancellationToken);
            if (category == null)
            {
                throw new PaintingMustHaveAnAssignedCategoryException();
            }

            // Generate slug from title
            var paintingName = new PaintingName(title);
            var slug = PaintingSlug.FromTitle(paintingName);

            // Check if a painting with this slug already exists
            var exists = await _paintingReadRepository.ExistsBySlugAsync(slug, cancellationToken);
            if (exists)
            {
                throw new PaintingSlugAlreadyExistsException();
            }

            // Auto-generate ID
            var id = new PaintingID();

            // Create painting using synchronous factory (no async needed for pure domain logic)
            var painting = _factory.Create(
                id,
                paintingName,
                PaintingDescription.FromNullable(description),
                imageUrl,
                PaintingThumbnailUrl.FromNullable(thumbnailUrl),
                categorySlugVO,
                price,
                width,
                height,
                depth,
                year,
                isAvailable);

            await _paintingWriteRepository.AddAsync(painting, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return new PaintingCreatedResult(painting.Id, painting.Slug.Value, painting.Title.Value);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
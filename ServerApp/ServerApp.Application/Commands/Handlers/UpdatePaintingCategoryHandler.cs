namespace ServerApp.Application.Commands.Handlers;

using MediatR;
using ServerApp.Shared.Persistence;
using ServerApp.Application.Commands;
using ServerApp.Domain.Repositories.Write;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Domain.ValueObjects.PaintingCategory;
using ServerApp.Domain.Services;
using ServerApp.Application.Exceptions;

public class UpdatePaintingCategoryHandler : IRequestHandler<UpdatePaintingCategory>
{
    private readonly IPaintingCategoryWriteRepository _writeRepository;
    private readonly IPaintingCategoryReadRepository _readRepository;
    private readonly IPaintingWriteRepository _paintingWriteRepository;
    private readonly IPaintingReadRepository _paintingReadRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHtmlSanitizer _htmlSanitizer;

    public UpdatePaintingCategoryHandler(
        IPaintingCategoryWriteRepository writeRepository,
        IPaintingCategoryReadRepository readRepository,
        IPaintingWriteRepository paintingWriteRepository,
        IPaintingReadRepository paintingReadRepository,
        IUnitOfWork unitOfWork,
        IHtmlSanitizer htmlSanitizer)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _paintingWriteRepository = paintingWriteRepository;
        _paintingReadRepository = paintingReadRepository;
        _unitOfWork = unitOfWork;
        _htmlSanitizer = htmlSanitizer;
    }

    public async Task Handle(UpdatePaintingCategory command, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var category = await _readRepository.GetByIdAsync(command.Id, cancellationToken);
            if (category == null)
            {
                throw new PaintingCategoryNotFoundException(command.Id.ToString());
            }

            var oldSlug = category.Slug.Value;

            // Sanitize the description to prevent XSS attacks
            var sanitizedDescription = command.Description != null ? _htmlSanitizer.Sanitize(command.Description) : null;

            category.Update(
                command.Name != null ? new PaintingCategoryName(command.Name) : null,
                PaintingCategoryDescription.FromNullable(sanitizedDescription));

            await _writeRepository.UpdateAsync(category, cancellationToken);

            // Cascade slug change to all paintings in this category
            if (category.Slug.Value != oldSlug)
            {
                var paintings = await _paintingReadRepository.GetByCategoryAsync(new PaintingCategorySlug(oldSlug), cancellationToken);
                foreach (var painting in paintings)
                {
                    painting.UpdateCategorySlug(category.Slug);
                    await _paintingWriteRepository.UpdateAsync(painting, cancellationToken);
                }
            }

            await _unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}

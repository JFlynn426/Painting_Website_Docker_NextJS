namespace ServerApp.Application.Commands.Handlers;

using MediatR;
using ServerApp.Application.DTOs;
using ServerApp.Application.Commands;
using ServerApp.Application.Services;
using ServerApp.Shared.Persistence;
using ServerApp.Domain.Repositories.Write;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Domain.ValueObjects.PaintingCategory;
using ServerApp.Domain.Services;
using ServerApp.Application.Exceptions;

public class UpdatePaintingCategoryHandler : CommandHandlerBase, IRequestHandler<UpdatePaintingCategory, CommandCompletionResponse>
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
        IHtmlSanitizer htmlSanitizer,
        IConcurrencyLockService concurrencyLock,
        IIdempotencyKeyService idempotencyKey)
        : base(concurrencyLock, idempotencyKey)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _paintingWriteRepository = paintingWriteRepository;
        _paintingReadRepository = paintingReadRepository;
        _unitOfWork = unitOfWork;
        _htmlSanitizer = htmlSanitizer;
    }

    public async Task<CommandCompletionResponse> Handle(UpdatePaintingCategory command, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(command.AdminId, command.IdempotencyKey, async ct =>
        {
            await _unitOfWork.BeginTransactionAsync(ct);

            try
            {
                var category = await _writeRepository.GetByIdAsync(command.Id, ct);
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

                await _writeRepository.UpdateAsync(category, ct);

                // Cascade slug change to all paintings in this category
                int affectedCount = 1;
                if (category.Slug.Value != oldSlug)
                {
                    var paintings = await _paintingWriteRepository.GetByCategoryAsync(new PaintingCategorySlug(oldSlug), ct);
                    foreach (var painting in paintings)
                    {
                        painting.UpdateCategorySlug(category.Slug);
                        await _paintingWriteRepository.UpdateAsync(painting, ct);
                        affectedCount++;
                    }
                }

                await _unitOfWork.CommitAsync(ct);
                return affectedCount;
            }
            catch
            {
                await _unitOfWork.RollbackAsync(ct);
                throw;
            }
        }, cancellationToken);
    }
}

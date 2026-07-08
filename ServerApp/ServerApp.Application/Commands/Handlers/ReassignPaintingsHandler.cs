namespace ServerApp.Application.Commands.Handlers;

using MediatR;
using ServerApp.Application.DTOs;
using ServerApp.Application.Commands;
using ServerApp.Application.Services;
using ServerApp.Shared.Persistence;
using ServerApp.Domain.Repositories.Write;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Application.Exceptions;

public class ReassignPaintingsHandler : CommandHandlerBase, IRequestHandler<ReassignPaintings, CommandCompletionResponse>
{
    private readonly IPaintingWriteRepository _paintingWriteRepository;
    private readonly IPaintingReadRepository _paintingReadRepository;
    private readonly IPaintingCategoryReadRepository _categoryReadRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReassignPaintingsHandler(
        IPaintingWriteRepository paintingWriteRepository,
        IPaintingReadRepository paintingReadRepository,
        IPaintingCategoryReadRepository categoryReadRepository,
        IUnitOfWork unitOfWork,
        IConcurrencyLockService concurrencyLock,
        IIdempotencyKeyService idempotencyKey)
        : base(concurrencyLock, idempotencyKey)
    {
        _paintingWriteRepository = paintingWriteRepository;
        _paintingReadRepository = paintingReadRepository;
        _categoryReadRepository = categoryReadRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CommandCompletionResponse> Handle(ReassignPaintings command, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(command.AdminId, command.IdempotencyKey, async ct =>
        {
            await _unitOfWork.BeginTransactionAsync(ct);

            try
            {
                // Load all unique categories upfront
                var categoryIds = command.PaintingIdToCategoryId.Values.Distinct().ToList();
                var categories = new Dictionary<Guid, Domain.Entities.PaintingCategory>();
                foreach (var categoryId in categoryIds)
                {
                    var category = await _categoryReadRepository.GetByIdAsync(categoryId, ct);
                    if (category == null)
                    {
                        throw new PaintingCategoryNotFoundException(categoryId.ToString());
                    }
                    categories[categoryId] = category;
                }

                // Reassign each painting to its target category
                int affectedCount = 0;
                foreach (var kvp in command.PaintingIdToCategoryId)
                {
                    var painting = await _paintingWriteRepository.GetByIdAsync(kvp.Key, ct);
                    if (painting != null)
                    {
                        painting.AssignCategory(categories[kvp.Value]);
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

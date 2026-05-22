namespace ServerApp.Application.Commands.Handlers;

using MediatR;
using ServerApp.Application.DTOs;
using ServerApp.Application.Commands;
using ServerApp.Application.Services;
using ServerApp.Shared.Persistence;
using ServerApp.Domain.Repositories.Write;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Application.Exceptions;

public class AssignPaintingCategoryHandler : CommandHandlerBase, IRequestHandler<AssignPaintingCategory, CommandCompletionResponse>
{
    private readonly IPaintingWriteRepository _paintingWriteRepository;
    private readonly IPaintingReadRepository _paintingReadRepository;
    private readonly IPaintingCategoryReadRepository _categoryReadRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignPaintingCategoryHandler(
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

    public async Task<CommandCompletionResponse> Handle(AssignPaintingCategory command, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(command.AdminId, command.IdempotencyKey, async ct =>
        {
            await _unitOfWork.BeginTransactionAsync(ct);

            try
            {
                var painting = await _paintingReadRepository.GetByIdAsync(command.PaintingId, ct);
                if (painting == null)
                {
                    throw new PaintingNotFoundException(command.PaintingId.ToString());
                }

                var category = await _categoryReadRepository.GetByIdAsync(command.CategoryId, ct);
                if (category == null)
                {
                    throw new PaintingCategoryNotFoundException(command.CategoryId.ToString());
                }

                painting.AssignCategory(category);

                await _paintingWriteRepository.UpdateAsync(painting, ct);
                await _unitOfWork.CommitAsync(ct);
                return 1;
            }
            catch
            {
                await _unitOfWork.RollbackAsync(ct);
                throw;
            }
        }, cancellationToken);
    }
}

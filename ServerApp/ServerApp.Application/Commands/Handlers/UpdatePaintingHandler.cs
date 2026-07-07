namespace ServerApp.Application.Commands.Handlers;

using MediatR;
using ServerApp.Application.DTOs;
using ServerApp.Application.Commands;
using ServerApp.Application.Services;
using ServerApp.Shared.Persistence;
using ServerApp.Domain.Repositories.Write;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Domain.ValueObjects.Painting;
using ServerApp.Application.Exceptions;

public class UpdatePaintingHandler : CommandHandlerBase, IRequestHandler<UpdatePainting, CommandCompletionResponse>
{
    private readonly IPaintingWriteRepository _writeRepository;
    private readonly IPaintingReadRepository _readRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePaintingHandler(
        IPaintingWriteRepository writeRepository,
        IPaintingReadRepository readRepository,
        IUnitOfWork unitOfWork,
        IConcurrencyLockService concurrencyLock,
        IIdempotencyKeyService idempotencyKey)
        : base(concurrencyLock, idempotencyKey)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CommandCompletionResponse> Handle(UpdatePainting command, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(command.AdminId, command.IdempotencyKey, async ct =>
        {
            await _unitOfWork.BeginTransactionAsync(ct);

            try
            {
                var painting = await _readRepository.GetByIdAsync(command.Id, ct);
                if (painting == null)
                {
                    throw new PaintingNotFoundException(command.Id.ToString());
                }

                painting.Update(
                    PaintingDescription.FromNullable(command.Description),
                    command.ImageUrl != null ? new PaintingImageUrl(command.ImageUrl) : null,
                    PaintingThumbnailUrl.FromNullable(command.ThumbnailUrl),
                    command.Price != null ? new PaintingPrice(command.Price.Value) : null,
                    command.Width != null ? new PaintingWidth(command.Width.Value) : null,
                    command.Height != null ? new PaintingHeight(command.Height.Value) : null,
                    command.Depth != null ? new PaintingDepth(command.Depth.Value) : null,
                    command.Year != null ? new PaintingYear(command.Year.Value) : null,
                    command.IsAvailable != null ? new PaintingIsAvailable(command.IsAvailable.Value) : null,
                    command.IsNew != null ? new PaintingIsNew(command.IsNew.Value) : null);
                // NOTE: isLandscape is NOT updated here - it is a derived property from the image file
                // and should only be set during image upload via ImageProcessingService

                await _writeRepository.UpdateAsync(painting, ct);
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

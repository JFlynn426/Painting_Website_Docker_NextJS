namespace ServerApp.Application.Commands.Handlers;

using MediatR;
using ServerApp.Shared.Persistence;
using ServerApp.Application.Commands;
using ServerApp.Domain.Repositories.Write;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Domain.ValueObjects.Painting;
using ServerApp.Application.Exceptions;

public class UpdatePaintingHandler : IRequestHandler<UpdatePainting>
{
    private readonly IPaintingWriteRepository _writeRepository;
    private readonly IPaintingReadRepository _readRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePaintingHandler(
        IPaintingWriteRepository writeRepository,
        IPaintingReadRepository readRepository,
        IUnitOfWork unitOfWork)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdatePainting command, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var painting = await _readRepository.GetByIdAsync(command.Id, cancellationToken);
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
                command.IsNew != null ? new PaintingIsNew(command.IsNew.Value) : null,
                command.IsCarouselPainting != null ? new PaintingIsCarouselPainting(command.IsCarouselPainting.Value) : null);

            await _writeRepository.UpdateAsync(painting, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}

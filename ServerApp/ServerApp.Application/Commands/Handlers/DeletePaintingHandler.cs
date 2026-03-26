namespace ServerApp.Application.Commands.Handlers;

using MediatR;
using ServerApp.Shared.Persistence;
using ServerApp.Application.Commands;
using ServerApp.Domain.Repositories.Write;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Application.Exceptions;

public class DeletePaintingHandler : IRequestHandler<DeletePainting>
{
    private readonly IPaintingWriteRepository _writeRepository;
    private readonly IPaintingReadRepository _readRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePaintingHandler(
        IPaintingWriteRepository writeRepository,
        IPaintingReadRepository readRepository,
        IUnitOfWork unitOfWork)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeletePainting command, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Get all paintings and find by ID
            var allPaintings = await _readRepository.GetAllAsync(cancellationToken);
            var painting = allPaintings.FirstOrDefault(p => p.Id == command.Id);

            if (painting == null)
            {
                throw new PaintingNotFoundException(command.Id.ToString());
            }

            painting.MarkAsDeleted();
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
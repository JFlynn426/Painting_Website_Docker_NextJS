namespace ServerApp.Application.Commands.Handlers;

using MediatR;
using ServerApp.Shared.Persistence;
using ServerApp.Application.Commands;
using ServerApp.Domain.Repositories.Write;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Application.Exceptions;

public class DeletePaintingCategoryHandler : IRequestHandler<DeletePaintingCategory>
{
    private readonly IPaintingCategoryWriteRepository _writeRepository;
    private readonly IPaintingCategoryReadRepository _readRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePaintingCategoryHandler(
        IPaintingCategoryWriteRepository writeRepository,
        IPaintingCategoryReadRepository readRepository,
        IUnitOfWork unitOfWork)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeletePaintingCategory command, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var category = await _readRepository.GetAsync(command.Id, cancellationToken);
            if (category == null)
            {
                throw new PaintingCategoryNotFoundException(command.Id.ToString());
            }

            await _writeRepository.DeleteAsync(command.Id, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
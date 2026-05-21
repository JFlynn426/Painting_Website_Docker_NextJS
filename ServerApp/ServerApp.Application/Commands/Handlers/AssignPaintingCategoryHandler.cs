namespace ServerApp.Application.Commands.Handlers;

using MediatR;
using ServerApp.Shared.Persistence;
using ServerApp.Application.Commands;
using ServerApp.Domain.Repositories.Write;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Application.Exceptions;

public class AssignPaintingCategoryHandler : IRequestHandler<AssignPaintingCategory>
{
    private readonly IPaintingWriteRepository _paintingWriteRepository;
    private readonly IPaintingReadRepository _paintingReadRepository;
    private readonly IPaintingCategoryReadRepository _categoryReadRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignPaintingCategoryHandler(
        IPaintingWriteRepository paintingWriteRepository,
        IPaintingReadRepository paintingReadRepository,
        IPaintingCategoryReadRepository categoryReadRepository,
        IUnitOfWork unitOfWork)
    {
        _paintingWriteRepository = paintingWriteRepository;
        _paintingReadRepository = paintingReadRepository;
        _categoryReadRepository = categoryReadRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AssignPaintingCategory command, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var painting = await _paintingReadRepository.GetByIdAsync(command.PaintingId, cancellationToken);
            if (painting == null)
            {
                throw new PaintingNotFoundException(command.PaintingId.ToString());
            }

            var category = await _categoryReadRepository.GetByIdAsync(command.CategoryId, cancellationToken);
            if (category == null)
            {
                throw new PaintingCategoryNotFoundException(command.CategoryId.ToString());
            }

            painting.AssignCategory(category);

            await _paintingWriteRepository.UpdateAsync(painting, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}

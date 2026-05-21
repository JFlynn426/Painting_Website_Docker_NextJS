namespace ServerApp.Application.Commands.Handlers;

using MediatR;
using ServerApp.Shared.Persistence;
using ServerApp.Application.Commands;
using ServerApp.Domain.Repositories.Write;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Application.Exceptions;

public class ReassignPaintingsHandler : IRequestHandler<ReassignPaintings>
{
    private readonly IPaintingWriteRepository _paintingWriteRepository;
    private readonly IPaintingReadRepository _paintingReadRepository;
    private readonly IPaintingCategoryReadRepository _categoryReadRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReassignPaintingsHandler(
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

    public async Task Handle(ReassignPaintings command, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Load all unique categories upfront
            var categoryIds = command.PaintingIdToCategoryId.Values.Distinct().ToList();
            var categories = new Dictionary<Guid, Domain.Entities.PaintingCategory>();
            foreach (var categoryId in categoryIds)
            {
                var category = await _categoryReadRepository.GetByIdAsync(categoryId, cancellationToken);
                if (category == null)
                {
                    throw new PaintingCategoryNotFoundException(categoryId.ToString());
                }
                categories[categoryId] = category;
            }

            // Reassign each painting to its target category
            foreach (var kvp in command.PaintingIdToCategoryId)
            {
                var painting = await _paintingReadRepository.GetByIdAsync(kvp.Key, cancellationToken);
                if (painting != null)
                {
                    painting.AssignCategory(categories[kvp.Value]);
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

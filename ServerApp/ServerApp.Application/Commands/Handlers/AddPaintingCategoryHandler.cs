namespace ServerApp.Application.Commands.Handlers;

using MediatR;
using ServerApp.Shared.Persistence;
using ServerApp.Application.Commands;
using ServerApp.Application.DTOs;
using ServerApp.Domain.Factories;
using ServerApp.Domain.Repositories.Write;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Domain.ValueObjects.PaintingCategory;
using ServerApp.Application.Exceptions;

public class AddPaintingCategoryHandler : IRequestHandler<AddPaintingCategory, PaintingCategoryCreatedResult>
{
    private readonly IPaintingCategoryFactory _factory;
    private readonly IPaintingCategoryWriteRepository _writeRepository;
    private readonly IPaintingCategoryReadRepository _readRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddPaintingCategoryHandler(
        IPaintingCategoryFactory factory,
        IPaintingCategoryWriteRepository writeRepository,
        IPaintingCategoryReadRepository readRepository,
        IUnitOfWork unitOfWork)
    {
        _factory = factory;
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PaintingCategoryCreatedResult> Handle(AddPaintingCategory command, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var (name, description) = command;

            // Create category using factory (factory handles ID and slug generation internally)
            var category = await _factory.CreateAsync(
                new PaintingCategoryName(name),
                PaintingCategoryDescription.FromNullable(description),
                cancellationToken);

            await _writeRepository.AddAsync(category, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return new PaintingCategoryCreatedResult(category.Id, category.Slug.Value, category.Name.Value);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
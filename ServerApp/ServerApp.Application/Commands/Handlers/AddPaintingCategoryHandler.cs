namespace ServerApp.Application.Commands.Handlers;

using MediatR;
using ServerApp.Shared.Persistence;
using ServerApp.Application.Commands;
using ServerApp.Application.DTOs;
using ServerApp.Domain.Factories;
using ServerApp.Domain.Repositories.Write;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Domain.Services;
using ServerApp.Application.Exceptions;

public class AddPaintingCategoryHandler : IRequestHandler<AddPaintingCategory, PaintingCategoryCreatedResult>
{
    private readonly IPaintingCategoryFactory _factory;
    private readonly IPaintingCategoryWriteRepository _writeRepository;
    private readonly IPaintingCategoryReadRepository _readRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHtmlSanitizer _htmlSanitizer;

    public AddPaintingCategoryHandler(
        IPaintingCategoryFactory factory,
        IPaintingCategoryWriteRepository writeRepository,
        IPaintingCategoryReadRepository readRepository,
        IUnitOfWork unitOfWork,
        IHtmlSanitizer htmlSanitizer)
    {
        _factory = factory;
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _unitOfWork = unitOfWork;
        _htmlSanitizer = htmlSanitizer;
    }

    public async Task<PaintingCategoryCreatedResult> Handle(AddPaintingCategory command, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var (name, description) = command;

            // Sanitize the description to prevent XSS attacks
            var sanitizedDescription = description != null ? _htmlSanitizer.Sanitize(description) : null;

            // Create category using factory (factory handles ID and slug generation internally)
            var category = await _factory.CreateAsync(
                name,
                sanitizedDescription,
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
namespace ServerApp.Application.Commands.Handlers;

using MediatR;
using ServerApp.Shared.Persistence;
using ServerApp.Application.Commands;
using ServerApp.Application.DTOs;
using ServerApp.Domain.Factories;
using ServerApp.Domain.Repositories.Write;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Domain.ValueObjects.Page;
using ServerApp.Domain.Exceptions;

public class AddPageContentHandler : IRequestHandler<AddPageContent, PageContentCreatedResult>
{
    private readonly IPageContentFactory _factory;
    private readonly IPageContentWriteRepository _writeRepository;
    private readonly IPageContentReadRepository _readRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddPageContentHandler(
        IPageContentFactory factory,
        IPageContentWriteRepository writeRepository,
        IPageContentReadRepository readRepository,
        IUnitOfWork unitOfWork)
    {
        _factory = factory;
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PageContentCreatedResult> Handle(AddPageContent command, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var (address, title, content) = command;

            // Check if page content already exists
            var exists = await _readRepository.ExistsByAddressAsync(new PageAddress(address), cancellationToken);
            if (exists)
            {
                throw new PageContentAlreadyExistsException(address);
            }

            // Create page content using factory
            var pageContent = _factory.Create(
                new PageAddress(address),
                PageTitle.FromNullable(title),
                new PageContentText(content));

            await _writeRepository.AddAsync(pageContent, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return new PageContentCreatedResult(pageContent.Id, pageContent.Address.Value, pageContent.Title?.Value);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
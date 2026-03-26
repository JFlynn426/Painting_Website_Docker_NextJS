namespace ServerApp.Application.Commands.Handlers;

using MediatR;
using ServerApp.Shared.Persistence;
using ServerApp.Application.Commands;
using ServerApp.Domain.Repositories.Write;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Domain.ValueObjects.Page;
using ServerApp.Application.Exceptions;

public class DeletePageContentHandler : IRequestHandler<DeletePageContent>
{
    private readonly IPageContentWriteRepository _writeRepository;
    private readonly IPageContentReadRepository _readRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePageContentHandler(
        IPageContentWriteRepository writeRepository,
        IPageContentReadRepository readRepository,
        IUnitOfWork unitOfWork)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeletePageContent command, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var pageContent = await _readRepository.GetByAddressAsync(new PageAddress(command.Address), cancellationToken);
            if (pageContent == null)
            {
                throw new PageContentNotFoundException(command.Address);
            }

            await _writeRepository.DeleteAsync(pageContent.Id, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
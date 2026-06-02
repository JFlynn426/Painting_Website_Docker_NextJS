namespace ServerApp.Application.Commands.Handlers;

using MediatR;
using ServerApp.Application.DTOs;
using ServerApp.Application.Commands;
using ServerApp.Application.Services;
using ServerApp.Shared.Persistence;
using ServerApp.Domain.Repositories.Write;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Domain.ValueObjects.Page;
using ServerApp.Application.Exceptions;

public class UpdatePageContentHandler : CommandHandlerBase, IRequestHandler<UpdatePageContent, CommandCompletionResponse>
{
    private readonly IPageContentWriteRepository _writeRepository;
    private readonly IPageContentReadRepository _readRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePageContentHandler(
        IPageContentWriteRepository writeRepository,
        IPageContentReadRepository readRepository,
        IUnitOfWork unitOfWork,
        IConcurrencyLockService concurrencyLock,
        IIdempotencyKeyService idempotencyKey)
        : base(concurrencyLock, idempotencyKey)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CommandCompletionResponse> Handle(UpdatePageContent command, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(command.AdminId, command.IdempotencyKey, async ct =>
        {
            await _unitOfWork.BeginTransactionAsync(ct);

            try
            {
                var pageContent = await _readRepository.GetByIdAsync(command.Id, ct);
                if (pageContent == null)
                {
                    throw new PageContentNotFoundException(command.Id.ToString());
                }

                pageContent.Update(
                    PageTitle.FromNullable(command.Title),
                    PageContentText.FromNullable(command.Content),
                    PagePhotoUrl.FromNullable(command.PhotoUrl));

                await _writeRepository.UpdateAsync(pageContent, ct);
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

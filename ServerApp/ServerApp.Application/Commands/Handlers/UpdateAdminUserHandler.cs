namespace ServerApp.Application.Commands.Handlers;

using MediatR;
using ServerApp.Application.DTOs;
using ServerApp.Application.Commands;
using ServerApp.Application.Services;
using ServerApp.Shared.Persistence;
using ServerApp.Domain.Repositories.Write;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Domain.ValueObjects.Admin;

public class UpdateAdminUserHandler : CommandHandlerBase, IRequestHandler<UpdateAdminUser, CommandCompletionResponse>
{
    private readonly IAdminUserWriteRepository _writeRepository;
    private readonly IAdminUserReadRepository _readRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAdminUserHandler(
        IAdminUserWriteRepository writeRepository,
        IAdminUserReadRepository readRepository,
        IUnitOfWork unitOfWork,
        IConcurrencyLockService concurrencyLock,
        IIdempotencyKeyService idempotencyKey)
        : base(concurrencyLock, idempotencyKey)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CommandCompletionResponse> Handle(UpdateAdminUser command, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(command.AdminId, command.IdempotencyKey, async ct =>
        {
            await _unitOfWork.BeginTransactionAsync(ct);

            try
            {
                var adminUser = await _readRepository.GetByIdAsync(command.Id, ct);
                if (adminUser == null)
                {
                    throw new InvalidOperationException($"Admin user with ID {command.Id} not found.");
                }

                adminUser.Update(
                    command.DisplayName != null ? new AdminName(command.DisplayName) : null,
                    AdminPictureUrl.FromNullable(command.PictureUrl),
                    command.IsActive != null ? new AdminIsActive(command.IsActive.Value) : null);

                await _writeRepository.UpdateAsync(adminUser, ct);
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

namespace ServerApp.Application.Commands.Handlers;

using MediatR;
using ServerApp.Shared.Persistence;
using ServerApp.Application.Commands;
using ServerApp.Domain.Repositories.Write;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Domain.ValueObjects.Admin;

public class UpdateAdminUserHandler : IRequestHandler<UpdateAdminUser>
{
    private readonly IAdminUserWriteRepository _writeRepository;
    private readonly IAdminUserReadRepository _readRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAdminUserHandler(
        IAdminUserWriteRepository writeRepository,
        IAdminUserReadRepository readRepository,
        IUnitOfWork unitOfWork)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateAdminUser command, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var adminUser = await _readRepository.GetByIdAsync(command.Id, cancellationToken);
            if (adminUser == null)
            {
                throw new InvalidOperationException($"Admin user with ID {command.Id} not found.");
            }

            adminUser.Update(
                command.DisplayName != null ? new AdminName(command.DisplayName) : null,
                AdminPictureUrl.FromNullable(command.PictureUrl),
                command.IsActive != null ? new AdminIsActive(command.IsActive.Value) : null);

            await _writeRepository.UpdateAsync(adminUser, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ServerApp.Shared.Abstractions.Commands
{
    public interface ICommandHandler<TCommand> where TCommand : class, ICommand
    {
        Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
    }
}

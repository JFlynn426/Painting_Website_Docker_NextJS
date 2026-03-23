using System;
using System.Collections.Generic;
using System.Text;

namespace ServerApp.Shared.Abstractions.Commands
{
    public interface ICommandDispatcher
    {
        Task DispatchAsync<TCommand>(TCommand command) where TCommand : class,ICommand;
    }
}

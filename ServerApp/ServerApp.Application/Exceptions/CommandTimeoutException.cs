namespace ServerApp.Application.Exceptions;

using ServerApp.Shared.Exceptions;

public class CommandTimeoutException : ServerAppException
{
    public CommandTimeoutException(string message = "Command execution timed out after 90 seconds. Please try again.")
        : base(message)
    {
    }
}

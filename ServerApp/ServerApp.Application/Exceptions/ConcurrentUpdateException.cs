namespace ServerApp.Application.Exceptions;

using ServerApp.Shared.Exceptions;

public class ConcurrentUpdateException : ServerAppException
{
    public ConcurrentUpdateException(string message = "Another update is currently in progress. Please try again later.")
        : base(message)
    {
    }
}

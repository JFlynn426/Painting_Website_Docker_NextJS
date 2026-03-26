using ServerApp.Shared.Exceptions;

namespace ServerApp.Domain.Exceptions;

public class PageContentAlreadyExistsException : ServerAppException
{
    public PageContentAlreadyExistsException(string address)
        : base($"Page content with address '{address}' already exists.")
    {
    }
}
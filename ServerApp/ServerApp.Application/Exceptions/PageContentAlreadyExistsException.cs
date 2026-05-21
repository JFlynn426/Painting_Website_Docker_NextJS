namespace ServerApp.Application.Exceptions;

using ServerApp.Shared.Exceptions;

public class PageContentAlreadyExistsException : ServerAppException
{
    public PageContentAlreadyExistsException(string address)
        : base($"Page content with address '{address}' already exists.")
    {
    }
}

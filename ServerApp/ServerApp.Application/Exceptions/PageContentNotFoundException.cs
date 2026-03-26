namespace ServerApp.Application.Exceptions;

using ServerApp.Shared.Exceptions;

public class PageContentNotFoundException : ServerAppException
{
    public PageContentNotFoundException(string address)
        : base($"Page content with address '{address}' not found.")
    {
    }
}
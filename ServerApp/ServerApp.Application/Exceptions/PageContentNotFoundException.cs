namespace ServerApp.Application.Exceptions;

using ServerApp.Shared.Abstractions.Exceptions;

public class PageContentNotFoundException : ServerAppException
{
    public PageContentNotFoundException(string address)
        : base($"Page content with address '{address}' not found.")
    {
    }
}
namespace ServerApp.Domain.Exceptions;

using ServerApp.Shared.Exceptions;

public class InvalidUrlFormatException : ServerAppException
{
    public InvalidUrlFormatException(string url)
        : base($"Invalid URL format: {url}")
    {
    }
}
namespace ServerApp.Application.Exceptions;

using ServerApp.Shared.Exceptions;

public class PaintingNotFoundException : ServerAppException
{
    public PaintingNotFoundException(string slug)
        : base($"Painting with slug '{slug}' not found.")
    {
    }
}
namespace ServerApp.Application.Exceptions;

using ServerApp.Shared.Abstractions.Exceptions;

public class PaintingNotFoundException : ServerAppException
{
    public PaintingNotFoundException(Guid id)
        : base($"Painting with id '{id}' not found.")
    {
    }
}
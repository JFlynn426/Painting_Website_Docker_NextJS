namespace ServerApp.Application.Exceptions;

using ServerApp.Shared.Abstractions.Exceptions;

public class PaintingCategoryNotFoundException : ServerAppException
{
    public PaintingCategoryNotFoundException(string slug)
        : base($"Painting category with slug '{slug}' not found.")
    {
    }
}
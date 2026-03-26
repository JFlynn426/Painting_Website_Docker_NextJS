namespace ServerApp.Application.Exceptions;

using ServerApp.Shared.Exceptions;

public class PaintingCategoryNotFoundException : ServerAppException
{
    public PaintingCategoryNotFoundException(string slug)
        : base($"Painting category with slug '{slug}' not found.")
    {
    }
}
namespace ServerApp.Domain.Exceptions;

using ServerApp.Shared.Exceptions;

public class PaintingCategoryNameAlreadyExistsException : ServerAppException
{
    public PaintingCategoryNameAlreadyExistsException(string categoryName)
        : base($"A painting category with the name '{categoryName}' already exists.")
    {
    }
}

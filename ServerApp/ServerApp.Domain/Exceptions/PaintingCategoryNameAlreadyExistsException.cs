namespace ServerApp.Domain.Exceptions;

using ServerApp.Shared.Abstractions.Exceptions;

public class PaintingCategoryNameAlreadyExistsException : ServerAppException
{
    public PaintingCategoryNameAlreadyExistsException(string categoryName)
        : base($"A painting category with the name '{categoryName}' already exists.")
    {
    }
}

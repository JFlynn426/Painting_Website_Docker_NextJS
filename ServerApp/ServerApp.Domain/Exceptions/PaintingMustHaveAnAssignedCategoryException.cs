namespace ServerApp.Domain.Exceptions;

using ServerApp.Shared.Abstractions.Exceptions;

public class PaintingMustHaveAnAssignedCategoryException : ServerAppException
{
    public PaintingMustHaveAnAssignedCategoryException()
        : base("Painting must have an assigned category on creation")
    {
    }
}
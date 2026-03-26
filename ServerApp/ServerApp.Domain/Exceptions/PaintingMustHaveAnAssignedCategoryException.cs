namespace ServerApp.Domain.Exceptions;

using ServerApp.Shared.Exceptions;

public class PaintingMustHaveAnAssignedCategoryException : ServerAppException
{
    public PaintingMustHaveAnAssignedCategoryException()
        : base("Painting must have an assigned category on creation")
    {
    }
}
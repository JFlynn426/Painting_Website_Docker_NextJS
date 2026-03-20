namespace ServerApp.Domain.Exceptions;

using ServerApp.Shared.Abstractions.Exceptions;

public class PaintingCategoryNotFoundException : ServerAppException
{
    public PaintingCategoryNotFoundException()
        : base("A painting must be assigned to a category.")
    {
    }
}
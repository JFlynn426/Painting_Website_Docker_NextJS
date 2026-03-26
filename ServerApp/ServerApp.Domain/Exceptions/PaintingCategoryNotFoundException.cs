namespace ServerApp.Domain.Exceptions;

using ServerApp.Shared.Exceptions;

public class PaintingCategoryNotFoundException : ServerAppException
{
    public PaintingCategoryNotFoundException()
        : base("A painting must be assigned to a category.")
    {
    }
}
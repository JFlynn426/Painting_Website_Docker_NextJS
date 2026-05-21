namespace ServerApp.Application.Exceptions;

using ServerApp.Shared.Exceptions;

public class PaintingCategoryHasPaintingsException : ServerAppException
{
    public PaintingCategoryHasPaintingsException(string categoryName)
        : base($"Cannot delete painting category '{categoryName}' because it still contains paintings.")
    {
    }
}

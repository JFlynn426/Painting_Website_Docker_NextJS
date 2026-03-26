namespace ServerApp.Domain.Exceptions;

using ServerApp.Shared.Exceptions;
using ServerApp.Domain.ValueObjects.Painting;

public class PaintingYearOutOfRangeException : ServerAppException
{
    public PaintingYearOutOfRangeException(int year)
        : base($"Painting year must be between {PaintingYear.MinYear} and {PaintingYear.MaxYear}. Provided: {year}")
    {
    }
}
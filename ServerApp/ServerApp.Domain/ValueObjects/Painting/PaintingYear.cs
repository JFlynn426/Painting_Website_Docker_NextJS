namespace ServerApp.Domain.ValueObjects.Painting;

using ServerApp.Domain.Exceptions;

public record PaintingYear
{
    public const int MinYear = 1900;
    public const int MaxYear = 2100;

    public int Value { get; }

    public PaintingYear(int value)
    {
        if (value < MinYear || value > MaxYear)
        {
            throw new PaintingYearOutOfRangeException(value);
        }

        Value = value;
    }

    public static implicit operator PaintingYear(int year) => new(year);

    public static implicit operator int(PaintingYear year) => year.Value;
}
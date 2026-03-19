namespace ServerApp.Domain.ValueObjects;

using ServerApp.Shared.Abstractions.Domain;

public record PaintingCategoryDescription : StringValueObject
{
    public const int MaxLength = 2000;

    public PaintingCategoryDescription() : base()
    {
    }

    public PaintingCategoryDescription(string value) : base(value, MaxLength)
    {
    }

    public static implicit operator PaintingCategoryDescription(string description) => new(description);
}
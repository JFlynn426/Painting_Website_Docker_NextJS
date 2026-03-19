namespace ServerApp.Domain.ValueObjects;

using ServerApp.Shared.Abstractions.Domain;

public record PaintingCategoryName : StringValueObject
{
    public const int MaxLength = 200;

    public PaintingCategoryName() : base()
    {
    }

    public PaintingCategoryName(string value) : base(value, MaxLength)
    {
    }

    public static implicit operator PaintingCategoryName(string name) => new(name);
}
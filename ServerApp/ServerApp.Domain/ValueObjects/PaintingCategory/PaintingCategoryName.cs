namespace ServerApp.Domain.ValueObjects.PaintingCategory;

using ServerApp.Shared.Domain;

public record PaintingCategoryName : StringValueObject
{
    public const int MaxLength = 50;
    public const int MinLength = 3;

    public PaintingCategoryName() : base()
    {
    }

    public PaintingCategoryName(string value) : base(value, MaxLength, minLength: MinLength)
    {
    }

    public static implicit operator PaintingCategoryName(string name) => new(name);
}
namespace ServerApp.Domain.ValueObjects.PaintingCategory;

using ServerApp.Shared.Domain;

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

    public static PaintingCategoryDescription? FromNullable(string? value) => value != null ? new PaintingCategoryDescription(value) : null;
}
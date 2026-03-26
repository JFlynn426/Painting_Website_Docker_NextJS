namespace ServerApp.Domain.ValueObjects.Painting;

using ServerApp.Shared.Domain;

public record PaintingDescription : StringValueObject
{
    public const int MaxLength = 500;

    public PaintingDescription() : base()
    {
    }

    public PaintingDescription(string value) : base(value, MaxLength, allowEmpty: true)
    {
    }

    public static implicit operator PaintingDescription(string description) => new(description);

    public static PaintingDescription? FromNullable(string? value) => value != null ? new PaintingDescription(value) : null;
}
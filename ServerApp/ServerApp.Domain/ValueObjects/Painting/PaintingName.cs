namespace ServerApp.Domain.ValueObjects.Painting;

using ServerApp.Shared.Domain;

public record PaintingName : StringValueObject
{
    public const int MaxLength = 100;

    public PaintingName() : base()
    {
    }

    public PaintingName(string value) : base(value, MaxLength)
    {
    }

    public static implicit operator PaintingName(string name) => new(name);
}
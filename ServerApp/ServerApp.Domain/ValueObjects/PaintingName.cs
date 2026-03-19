namespace ServerApp.Domain.ValueObjects;

using ServerApp.Shared.Abstractions.Domain;

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
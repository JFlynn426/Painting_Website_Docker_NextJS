namespace ServerApp.Domain.ValueObjects.Painting;

using ServerApp.Shared.Abstractions.Domain;

public record PaintingDepth : DecimalValueObject
{
    public PaintingDepth() : base()
    {
    }

    public PaintingDepth(decimal value) : base(value, allowZero: false)
    {
    }

    public static implicit operator PaintingDepth(decimal depth) => new(depth);
}
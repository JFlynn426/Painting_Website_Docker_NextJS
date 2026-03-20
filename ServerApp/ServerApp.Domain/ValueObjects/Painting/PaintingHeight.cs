namespace ServerApp.Domain.ValueObjects.Painting;

using ServerApp.Shared.Abstractions.Domain;

public record PaintingHeight : DecimalValueObject
{
    public PaintingHeight() : base()
    {
    }

    public PaintingHeight(decimal value) : base(value, allowZero: false)
    {
    }

    public static implicit operator PaintingHeight(decimal height) => new(height);
}
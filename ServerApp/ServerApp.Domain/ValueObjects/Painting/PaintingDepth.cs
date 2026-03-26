namespace ServerApp.Domain.ValueObjects.Painting;

using ServerApp.Shared.Domain;

public record PaintingDepth : DecimalValueObject
{
    public PaintingDepth() : base()
    {
    }

    public PaintingDepth(decimal value) : base(value, allowZero: false)
    {
    }

    public static implicit operator PaintingDepth(decimal depth) => new(depth);

    public static PaintingDepth? FromNullable(decimal? value) => value != null ? new PaintingDepth(value.Value) : null;
}
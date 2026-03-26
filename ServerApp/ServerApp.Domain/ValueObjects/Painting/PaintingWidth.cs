namespace ServerApp.Domain.ValueObjects.Painting;

using ServerApp.Shared.Domain;

public record PaintingWidth : DecimalValueObject
{
    public PaintingWidth() : base()
    {
    }

    public PaintingWidth(decimal value) : base(value, allowZero: false)
    {
    }

    public static implicit operator PaintingWidth(decimal width) => new(width);

    public static PaintingWidth? FromNullable(decimal? value) => value != null ? new PaintingWidth(value.Value) : null;
}
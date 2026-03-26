namespace ServerApp.Domain.ValueObjects.Painting;

using ServerApp.Shared.Domain;

public record PaintingHeight : DecimalValueObject
{
    public PaintingHeight() : base()
    {
    }

    public PaintingHeight(decimal value) : base(value, allowZero: false)
    {
    }

    public static implicit operator PaintingHeight(decimal height) => new(height);

    public static PaintingHeight? FromNullable(decimal? value) => value != null ? new PaintingHeight(value.Value) : null;
}
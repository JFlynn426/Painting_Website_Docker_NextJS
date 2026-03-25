namespace ServerApp.Domain.ValueObjects.Painting;

using ServerApp.Shared.Abstractions.Domain;

public record PaintingPrice : DecimalValueObject
{
    public PaintingPrice() : base()
    {
    }

    public PaintingPrice(decimal value) : base(value, allowZero: true)
    {
    }

    public static implicit operator PaintingPrice(decimal price) => new(price);

    public static PaintingPrice? FromNullable(decimal? value) => value != null ? new PaintingPrice(value.Value) : null;
}
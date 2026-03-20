namespace ServerApp.Domain.ValueObjects.Painting;

public record PaintingIsAvailable
{
    public bool Value { get; }

    public PaintingIsAvailable(bool value)
    {
        Value = value;
    }

    public static implicit operator PaintingIsAvailable(bool isAvailable) => new(isAvailable);

    public static implicit operator bool(PaintingIsAvailable isAvailable) => isAvailable.Value;
}
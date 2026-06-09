namespace ServerApp.Domain.ValueObjects.Painting;

public record PaintingIsLandscape
{
    public bool Value { get; }

    public PaintingIsLandscape(bool value)
    {
        Value = value;
    }

    public static implicit operator PaintingIsLandscape(bool isLandscape) => new(isLandscape);

    public static implicit operator bool(PaintingIsLandscape isLandscape) => isLandscape.Value;
}

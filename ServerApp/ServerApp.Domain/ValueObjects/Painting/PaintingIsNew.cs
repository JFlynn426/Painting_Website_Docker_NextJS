namespace ServerApp.Domain.ValueObjects.Painting;

public record PaintingIsNew
{
    public bool Value { get; }

    public PaintingIsNew(bool value)
    {
        Value = value;
    }

    public static implicit operator PaintingIsNew(bool isNew) => new(isNew);

    public static implicit operator bool(PaintingIsNew isNew) => isNew.Value;
}
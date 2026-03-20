namespace ServerApp.Domain.ValueObjects.Painting;

public record PaintingID
{
    public Guid Value { get; }

    public PaintingID(Guid value)
    {
        Value = value;
    }

    public PaintingID() : this(Guid.NewGuid())
    {
    }

    public static implicit operator Guid(PaintingID id) => id.Value;

    public static implicit operator PaintingID(Guid id) => new(id);

    public static PaintingID New() => new(Guid.NewGuid());
}
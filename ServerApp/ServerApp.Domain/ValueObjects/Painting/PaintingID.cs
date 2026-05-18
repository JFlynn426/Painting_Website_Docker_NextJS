namespace ServerApp.Domain.ValueObjects.Painting;

using ServerApp.Shared.Domain;

public record PaintingID : GuidValueObject
{
    public PaintingID(Guid value) : base(value)
    {
    }

    public PaintingID() : base()
    {
    }

    public static implicit operator PaintingID(Guid id) => new(id);

    public static PaintingID New() => new(Guid.NewGuid());
}

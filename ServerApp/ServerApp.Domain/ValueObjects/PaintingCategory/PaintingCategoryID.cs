namespace ServerApp.Domain.ValueObjects.PaintingCategory;

public record PaintingCategoryID
{
    public Guid Value { get; }

    public PaintingCategoryID(Guid value)
    {
        Value = value;
    }

    public PaintingCategoryID() : this(Guid.NewGuid())
    {
    }

    public static implicit operator Guid(PaintingCategoryID id) => id.Value;

    public static implicit operator PaintingCategoryID(Guid id) => new(id);

    public static PaintingCategoryID New() => new(Guid.NewGuid());
}
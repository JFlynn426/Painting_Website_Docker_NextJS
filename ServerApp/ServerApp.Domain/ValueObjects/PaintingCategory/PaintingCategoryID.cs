namespace ServerApp.Domain.ValueObjects.PaintingCategory;

using ServerApp.Shared.Domain;

public record PaintingCategoryID : GuidValueObject
{
    public PaintingCategoryID(Guid value) : base(value)
    {
    }

    public PaintingCategoryID() : base()
    {
    }

    public static implicit operator PaintingCategoryID(Guid id) => new(id);

    public static PaintingCategoryID New() => new(Guid.NewGuid());
}

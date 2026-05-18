namespace ServerApp.Domain.ValueObjects.Admin;

using ServerApp.Shared.Domain;

public record AdminID : GuidValueObject
{
    public AdminID(Guid value) : base(value)
    {
    }

    public AdminID() : base()
    {
    }

    public static implicit operator AdminID(Guid id) => new(id);

    public static AdminID New() => new(Guid.NewGuid());
}

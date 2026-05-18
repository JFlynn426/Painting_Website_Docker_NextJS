namespace ServerApp.Domain.ValueObjects.Admin;

using ServerApp.Shared.Domain;

public record AdminName : StringValueObject
{
    public const int MaxLength = 100;

    public AdminName() : base()
    {
    }

    public AdminName(string value) : base(value, MaxLength)
    {
    }

    public static implicit operator AdminName(string name) => new(name);
}

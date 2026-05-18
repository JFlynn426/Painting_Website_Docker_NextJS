namespace ServerApp.Domain.ValueObjects.Admin;

using ServerApp.Shared.Domain;

public record AdminGoogleSub : StringValueObject
{
    public const int MaxLength = 100;

    public AdminGoogleSub() : base()
    {
    }

    public AdminGoogleSub(string value) : base(value, MaxLength)
    {
    }

    public static implicit operator AdminGoogleSub(string googleSub) => new(googleSub);
}

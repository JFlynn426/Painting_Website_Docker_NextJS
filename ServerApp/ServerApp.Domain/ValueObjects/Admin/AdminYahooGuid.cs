namespace ServerApp.Domain.ValueObjects.Admin;

using ServerApp.Shared.Domain;

public record AdminYahooGuid : StringValueObject
{
    public const int MaxLength = 100;

    public AdminYahooGuid() : base()
    {
    }

    public AdminYahooGuid(string value) : base(value, MaxLength)
    {
    }

    public static implicit operator AdminYahooGuid(string yahooGuid) => new(yahooGuid);
}

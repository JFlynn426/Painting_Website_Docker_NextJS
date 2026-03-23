namespace ServerApp.Domain.ValueObjects.Page;

using ServerApp.Shared.Abstractions.Domain;

public record PageAddress : StringValueObject
{
    public const int MaxLength = 50;

    public PageAddress() : base()
    {
    }

    public PageAddress(string value) : base(value, MaxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Address cannot be empty or whitespace.", nameof(value));
        }
    }

    public static implicit operator PageAddress(string address) => new(address);
}
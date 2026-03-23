namespace ServerApp.Domain.ValueObjects.Page;

using ServerApp.Shared.Abstractions.Domain;

public record PageTitle : StringValueObject
{
    public const int MaxLength = 100;

    public PageTitle() : base()
    {
    }

    public PageTitle(string value) : base(value, MaxLength)
    {
    }

    public static implicit operator PageTitle(string title) => new(title);
}
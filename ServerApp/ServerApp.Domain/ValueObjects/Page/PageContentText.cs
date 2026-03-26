namespace ServerApp.Domain.ValueObjects.Page;

using ServerApp.Shared.Domain;

public record PageContentText : StringValueObject
{
    public const int MaxLength = 10000;

    public PageContentText() : base()
    {
    }

    public PageContentText(string value) : base(value, MaxLength)
    {
    }

    public static implicit operator PageContentText(string content) => new(content);
}
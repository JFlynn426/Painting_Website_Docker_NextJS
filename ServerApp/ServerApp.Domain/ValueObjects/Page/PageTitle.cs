namespace ServerApp.Domain.ValueObjects.Page;

using ServerApp.Shared.Domain;

public record PageTitle : StringValueObject
{
    public const int MaxLength = 100;

    public PageTitle() : base()
    {
    }

    public PageTitle(string value) : base(value, MaxLength, allowEmpty: true)
    {
    }

    public static implicit operator PageTitle(string title) => new(title);

    public static PageTitle? FromNullable(string? value) => value != null ? new PageTitle(value) : null;
}
namespace ServerApp.Domain.ValueObjects.Page;

using ServerApp.Shared.Domain;

public record PagePhotoUrl : StringValueObject
{
    public const int MaxLength = 2000;

    public PagePhotoUrl() : base()
    {
    }

    public PagePhotoUrl(string value) : base(value, MaxLength, allowEmpty: true)
    {
    }

    public static implicit operator PagePhotoUrl(string value) => new(value);

    public static implicit operator string(PagePhotoUrl value) => value.Value;

    public static PagePhotoUrl? FromNullable(string? value) => value != null ? new PagePhotoUrl(value) : null;
}

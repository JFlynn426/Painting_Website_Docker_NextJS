namespace ServerApp.Domain.ValueObjects.Admin;

using ServerApp.Shared.Domain;

public record AdminPictureUrl : StringValueObject
{
    public const int MaxLength = 500;

    public AdminPictureUrl() : base()
    {
    }

    public AdminPictureUrl(string value) : base(value, MaxLength, allowEmpty: false)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out _))
        {
            throw new ArgumentException($"Invalid URL format: {value}", nameof(value));
        }
    }

    public static implicit operator AdminPictureUrl(string url) => new(url);

    public static AdminPictureUrl? FromNullable(string? url)
    {
        return url == null ? null : new AdminPictureUrl(url);
    }
}

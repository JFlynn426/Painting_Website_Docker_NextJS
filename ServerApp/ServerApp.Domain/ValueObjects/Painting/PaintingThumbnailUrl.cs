namespace ServerApp.Domain.ValueObjects.Painting;

using ServerApp.Shared.Domain;
using ServerApp.Shared.Validation;
using ServerApp.Domain.Exceptions;

public record PaintingThumbnailUrl : StringValueObject
{
    public PaintingThumbnailUrl() : base()
    {
    }

    public PaintingThumbnailUrl(string value) : base(value, int.MaxValue, allowEmpty: false, enforceMaxLength: false)
    {
        if (!UrlValidator.IsValid(value))
        {
            throw new InvalidUrlFormatException(value);
        }
    }

    public static implicit operator PaintingThumbnailUrl(string url) => new(url);

    public static PaintingThumbnailUrl? FromNullable(string? value) => value != null ? new PaintingThumbnailUrl(value) : null;
}
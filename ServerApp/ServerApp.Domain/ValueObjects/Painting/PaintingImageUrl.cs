namespace ServerApp.Domain.ValueObjects.Painting;

using ServerApp.Shared.Domain;
using ServerApp.Shared.Validation;
using ServerApp.Domain.Exceptions;

public record PaintingImageUrl : StringValueObject
{
    public const int MaxLength = 500;

    public PaintingImageUrl() : base()
    {
    }

    public PaintingImageUrl(string value) : base(value, MaxLength, allowEmpty: false)
    {
        if (!UrlValidator.IsValid(value))
        {
            throw new InvalidUrlFormatException(value);
        }
    }

    public static implicit operator PaintingImageUrl(string url) => new(url);
}
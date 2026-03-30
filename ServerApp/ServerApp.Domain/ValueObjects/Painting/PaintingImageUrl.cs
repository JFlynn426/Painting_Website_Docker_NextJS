namespace ServerApp.Domain.ValueObjects.Painting;

using ServerApp.Shared.Domain;

public record PaintingImageUrl : StringValueObject
{
    public const int MaxLength = 500;

    public PaintingImageUrl() : base()
    {
    }

    public PaintingImageUrl(string value) : base(value, MaxLength, allowEmpty: false)
    {
    }

    public static implicit operator PaintingImageUrl(string url) => new(url);
}
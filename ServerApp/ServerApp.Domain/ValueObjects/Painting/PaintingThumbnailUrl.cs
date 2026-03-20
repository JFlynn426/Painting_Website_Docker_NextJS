namespace ServerApp.Domain.ValueObjects.Painting;

using ServerApp.Shared.Abstractions.Domain;

public record PaintingThumbnailUrl : StringValueObject
{
    public PaintingThumbnailUrl() : base()
    {
    }

    public PaintingThumbnailUrl(string value) : base(value, int.MaxValue, allowEmpty: false, enforceMaxLength: false)
    {
    }

    public static implicit operator PaintingThumbnailUrl(string url) => new(url);
}
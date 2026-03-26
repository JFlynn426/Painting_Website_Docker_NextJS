namespace ServerApp.Domain.ValueObjects.Painting;

using ServerApp.Shared.Domain;

public record PaintingImageUrl : StringValueObject
{
    public PaintingImageUrl() : base()
    {
    }

    public PaintingImageUrl(string value) : base(value, int.MaxValue, allowEmpty: false, enforceMaxLength: false)
    {
    }

    public static implicit operator PaintingImageUrl(string url) => new(url);
}